<template>
  <q-dialog v-model="open" >
        <q-card>
            <form @submit.prevent="submit">
            <q-card-section class="row items-center q-pb-none">
                <div class="text-h6">Find...</div>
                <q-space />
                <q-btn icon="close" flat round dense v-close-popup />
            </q-card-section>

            <q-card-section>
                <div class="column" style="min-width: 340px">
                    <div class="col q-pa-sm">
                        <q-input outlined v-model="pattern" label="Find" autofocus />
                    </div>
                    <div class="col q-pa-sm">
                        <q-checkbox v-model="caseInsensitive" label="Case insensitive" color="primary" />
                    </div>
                    <div class="col q-pa-sm">
                        <q-checkbox v-model="regex" label="Use RegexExpression" color="primary" />
                    </div>
                    <div class="col q-pb-sm q-pl-sm">
                        <q-checkbox v-model="exact" label="Exact Match" color="primary" />
                    </div>
                    <div class="col q-pb-sm q-pl-sm">
                        <div class="row">
                            <div class="col-auto q-pr-sm">
                                Search Direction
                            </div>
                            <div class="col">
                                <q-radio v-model="direction" val="up" label="Up" />
                                <q-radio v-model="direction" val="down" label="Down" />
                            </div>
                        </div>
                    </div>
                </div>
            </q-card-section>
            <q-separator />
            <q-card-actions align="right">
                <q-btn v-close-popup flat>Close</q-btn>
                <q-btn class="q-ml-sm" color="primary" type="submit" unelevated
                    :disable="!pattern">Find</q-btn>
            </q-card-actions>
            </form>
        </q-card>
    </q-dialog>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Inject } from 'vue-property-decorator';
import { QVueGlobals, QSpinnerHourglass } from 'quasar';
import AppService from '../services/AppService';

@Component
export default class FindTool extends Vue {
    pattern: string ='';
    direction = 'down';
    directionOptions: any[] = [ { label: "Up", value: 'up'}, { label: 'Down', value: 'down'}];
    open: boolean =false;
    caseInsensitive:boolean = false;
    exact: boolean = false;
    regex: boolean =false;
    @Inject() appService!: AppService;
    $q!: QVueGlobals;

    mounted(){
        const showFindTools = this.show;
        const hideFindTools = this.hide;
        const isFindToolsOpen = ()=>{
            return this.open;
        }
        (window as any).$app = {
            ...(window as any).$app,
            showFindTools,
            hideFindTools,
            isFindToolsOpen
        }
    }

    submit(){
        let spinner = QSpinnerHourglass as any;
        this.$q.loading.show({
            spinner
        })
        this.appService.find(this.pattern, (window as any).$app.getSelectedIndex(), this.regex, this.caseInsensitive, this.exact, this.direction=='up'?'up':'down')
        .then(res=>{
            if (res.data && +res.data >=0){
                (window as any).$app.goTo(+res.data);
            }
            else {
                this.$q.dialog({
                    message: "Not matches found."
                });
            }
        })
        .finally(()=> this.$q.loading.hide())
    }

    show(){
        let text= this.getSelectionText();
        if (text)
            this.pattern = text;
        this.open= true;
    }

    hide(){
        this.open = false;
    }
    
    getSelectionText() {
        var text = "";
        var activeEl = document.activeElement as any;
        var activeElTagName = activeEl ? activeEl.tagName.toLowerCase() : null;
        if (
            (activeElTagName == "textarea") || (activeElTagName == "input" &&
            /^(?:text|search|password|tel|url)$/i.test(activeEl.type)) &&
            (typeof activeEl.selectionStart == "number")
            ) {
            text = activeEl.value.slice(activeEl.selectionStart, activeEl.selectionEnd);
        } else if (window.getSelection) {
            text = window.getSelection()?.toString() || "";
        }
        return text;
    }
}
</script>

<style>

</style>