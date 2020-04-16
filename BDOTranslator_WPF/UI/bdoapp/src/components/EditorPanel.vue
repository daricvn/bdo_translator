<template>
    <form @submit.prevent="submit">
        <div class="column full-height" style="min-height: 82vh">
            <div class="col-auto">
                <q-input class="full-width" outlined type="textarea" dense readonly v-model="oldText" />
            </div>
            <div class="col-auto text-center q-pa-sm">
                <q-btn class="q-pl-lg q-pr-lg" color="primary" unelevated icon="expand_more" @click="pullDown">
                    <q-tooltip content-style="font-size: 14px">
                        Duplicate (Ctrl + D)
                    </q-tooltip>
                </q-btn>
            </div>
            <div class="col-auto">
                <q-input class="full-width" outlined type="textarea" dense v-model="editorText" @keydown.enter.prevent @keyup.enter.prevent="submit" />
            </div>
            <div class="col">
                <slot></slot>
            </div>
            <div class="col-auto text-right q-pa-md" style="box-sizing: border-box">
                <q-btn outline class="q-ml-sm">
                    Exit
                </q-btn>
                <q-btn class="q-ml-sm" size="lg" color="positive" unelevated type="submit">
                    <q-icon class="on-left" name="save"></q-icon>
                    Save
                </q-btn>
            </div>
        </div>
    </form>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Prop, Emit, Watch, Inject } from 'vue-property-decorator';
import AppService from '../services/AppService';

@Component
export default class EditorPanel extends Vue{
    @Prop({ default: ''}) oldText: string = '';
    editorText: string = '';
    @Inject() appService!: AppService;
    mounted(){
        const duplicate = this.pullDown;
        (window as any).$app = {
            ...(window as any).$app,
            duplicate
        }
    }

    @Watch("oldText")
    clear(){
        this.editorText='';
    }

    @Emit("submit")
    submit(){
        return this.editorText;
    }

    pullDown(){
        this.editorText = this.oldText;
    }

    close(){
        
    }
}
</script>

<style>

</style>