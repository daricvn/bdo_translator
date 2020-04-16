<template>
  <q-dialog v-model="open" >
        <q-card>
            <form @submit.prevent="submit">
            <q-card-section class="row items-center q-pb-none">
                <div class="text-h6">Replace Tools</div>
                <q-space />
                <q-btn icon="close" flat round dense v-close-popup />
            </q-card-section>

            <q-card-section>
                <div class="column full-height" style="min-width: 300px">
                    <div class="col q-pa-sm">
                        <q-input outlined v-model="pattern" label="Find" />
                    </div>
                    <div class="col q-pa-sm">
                        <q-input outlined v-model="replaceTo" label="Replace to..." autofocus />
                    </div>
                    <div class="col q-pa-sm">
                        <q-checkbox v-model="caseInsensitive" label="Case insensitive" color="primary" />
                    </div>
                    <div class="col q-pa-sm">
                        <q-checkbox v-model="useRegex" label="Use RegexExpression" color="primary" />
                    </div>
                </div>
            </q-card-section>
            <q-separator />
            <q-card-actions align="right">
                <q-btn v-close-popup flat>Close</q-btn>
                <q-btn v-close-popup class="q-ml-sm" color="primary" type="submit" unelevated
                    :disable="!pattern">Replace All</q-btn>
            </q-card-actions>
            </form>
        </q-card>
    </q-dialog>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import AppService from '../services/AppService';
import { QVueGlobals } from 'quasar';
import { Inject } from 'vue-property-decorator';

@Component
export default class RegexTool extends Vue {
    pattern: string ='';
    replaceTo: string='';
    open: boolean =false;
    caseInsensitive:boolean = false;
    useRegex: boolean= false;
    @Inject() appService!: AppService;
    $q!: QVueGlobals;

    mounted(){
        const showRegexTools = this.show;
        const hideRegexTools = this.hide;
        const isRegexToolsOpen = ()=>{
            return this.open;
        }
        (window as any).$app = {
            ...(window as any).$app,
            showRegexTools,
            hideRegexTools,
            isRegexToolsOpen
        }        
    }

    submit(){
        this.$q.loading.show({
            message: 'Replacing...'
        })
        this.appService.regex(this.pattern, this.replaceTo, this.useRegex, this.caseInsensitive).then((res)=>{
            this.$q.loading.hide();
            if (res.data && !isNaN(+res.data)){
                let count = +res.data;
                this.$q.notify({
                    message: `Replaced ${count} rows.`,
                    color: 'primary',
                    position: 'top',
                    actions: [
                        { label: 'Dismiss', color: 'white' }
                    ]
                });
            }
        }).finally(()=>this.$q.loading.hide());
    }

    show(){
        this.open = true;
        this.pattern=this.getSelectionText();
        this.replaceTo='';
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