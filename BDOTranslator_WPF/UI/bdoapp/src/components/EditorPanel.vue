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
                <q-input ref="editor" class="full-width" outlined type="textarea" dense v-model="editorText" @keydown.enter="onKeyEnter" />
            </div>
            <div class="col-auto text-right q-pa-sm">
                <q-btn color="primary" unelevated icon="delete" @click="clear">
                    <q-tooltip content-style="font-size: 14px">
                        Clear (Ctrl + N)
                    </q-tooltip>
                </q-btn>
            </div>
            <div class="col">
                <slot></slot>
            </div>
            <div class="col-auto text-right q-pa-md" style="box-sizing: border-box">
                <q-btn outline class="q-ml-sm" @click="close">
                    Exit
                </q-btn>
                <q-btn outline class="q-ml-sm" color="primary" @click="undo">
                    <q-icon name="undo" class="on-left" />
                    Undo
                </q-btn>
                <q-btn outline class="q-ml-sm" color="secondary" @click="redo">
                    <q-icon name="redo" class="on-left" />
                    Redo
                </q-btn>
                <q-btn class="q-ml-sm" size="lg" color="positive" unelevated type="submit">
                    <q-icon class="on-left" name="save"></q-icon>
                    Save <span class="text-caption" style="font-size: 12px">(Shift + Enter)</span>
                </q-btn>
            </div>
            <div class="col-auto text-right">
                Developed by Darick (Ryu™)
            </div>
        </div>
    </form>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Prop, Emit, Watch, Inject, Ref } from 'vue-property-decorator';
import AppService from '../services/AppService';
import { QInput } from 'quasar';

@Component
export default class EditorPanel extends Vue{
    @Prop({ default: ''}) oldText: string = '';
    editorText: string = '';
    @Inject() appService!: AppService;
    @Ref("editor") editor!: QInput;
    mounted(){
        const duplicate = this.pullDown;
        const clearText = this.clear;
        const closeApp = this.close;
        (window as any).$app = {
            ...(window as any).$app,
            duplicate,
            clearText,
            closeApp
        }
    }


    clear(){
        this.editorText='';
        if (this.editor){
            this.editor.focus();
        }
    }

    @Emit("submit")
    submit(){
        return this.editorText;
    }

    @Watch("oldText")
    pullDown(){
        this.editorText = this.oldText;
    }

    onKeyEnter(e:KeyboardEvent){
        if (e.shiftKey){
            e.preventDefault();
            this.submit();
        }
    }

    close(){
        this.appService.closeApp();
    }

    undo(){
        this.appService.undo();
    }
    redo(){
        this.appService.redo();
    }
}
</script>

<style>

</style>