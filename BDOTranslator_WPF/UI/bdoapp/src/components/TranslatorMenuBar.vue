<template>
    <div>
        <q-bar class="bg-blue text-white">
            <div class="cursor-pointer non-selectable q-pl-sm p-pr-sm">
                File
                <q-menu>
                <q-list dense style="min-width: 100px">
                    <q-item clickable v-close-popup @click="browse">
                        <q-item-section>Open...</q-item-section>
                    </q-item>
                    <q-item clickable v-close-popup @click="saveFile">
                        <q-item-section>Write To File (Ctrl + S)</q-item-section>
                    </q-item>

                    <q-separator />

                    <q-item clickable v-close-popup @click="closeApp">
                        <q-item-section>Quit</q-item-section>
                    </q-item>
                </q-list>
                </q-menu>
            </div>

            <div class="q-ml-md cursor-pointer non-selectable  q-pl-sm p-pr-sm">
                Tools
                <q-menu auto-close>
                <q-list dense style="min-width: 100px">
                    <q-item clickable @click="showFindingTools">
                        <q-item-section>Find... (Ctrl + F)</q-item-section>
                    </q-item>
                    <q-item clickable @click="showRegexTools">
                        <q-item-section>Replace... (Ctrl + G)</q-item-section>
                    </q-item>
                    <q-separator />
                    <q-item clickable @click="showPatcherTools">
                        <q-item-section>Text Patcher</q-item-section>
                    </q-item>
                    <q-item clickable @click="showExtractorTools">
                        <q-item-section>.Loc Extractor</q-item-section>
                    </q-item>
                </q-list>
                </q-menu>
            </div>
            
            <div class="q-ml-md cursor-pointer non-selectable  q-pl-sm p-pr-sm" @click="aboutDialog.show()">
                About
            </div>
        </q-bar>
        <regex-tool />
        <find-tool />
        <loc-extractor-dialog />
        <patcher-tool />
        <about ref="about" />
    </div>
</template>

<script>
import Vue from 'vue'
import Component from 'vue-class-component';
import RegexTool from './RegexTool.vue';
import FindTool from './FindTool.vue';
import LocExtractorDialog from './LocExtractorDialog.vue';
import PatcherTool from './PatcherTool.vue';
import { Inject, Ref } from 'vue-property-decorator';
import AppService from '../services/AppService';
import About from '../views/About.vue';

@Component({
    components:{
        RegexTool,
        FindTool,
        LocExtractorDialog,
        PatcherTool,
        About
    }
})
export default class TranslatorMenuBar extends Vue {
    @Ref("about") aboutDialog;
    browse(){
        window.$app.browseFile();
    }

    closeApp(){
        window.$app.closeApp();
    }

    saveFile(){
        window.$app.saveFile();
    }

    showRegexTools(){
        window.$app.showRegexTools();
    }

    showFindingTools(){
        window.$app.showFindTools();
    }

    showExtractorTools(){
        window.$app.showExtractorTools();
    }

    showPatcherTools(){
        window.$app.showPatcherTools();
    }
}
</script>

<style>

</style>