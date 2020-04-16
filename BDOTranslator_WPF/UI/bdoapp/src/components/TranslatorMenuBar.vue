<template>
    <div>
        <q-bar class="bg-blue text-white">
            <div class="cursor-pointer non-selectable q-pl-sm p-pr-sm">
                File
                <q-menu>
                <q-list dense style="min-width: 100px">
                    <q-item clickable v-close-popup>
                    <q-item-section>Open...</q-item-section>
                    </q-item>
                    <q-item clickable v-close-popup>
                    <q-item-section>Write To File (Ctrl + S)</q-item-section>
                    </q-item>

                    <q-separator />

                    <q-item clickable v-close-popup>
                    <q-item-section>Quit</q-item-section>
                    </q-item>
                </q-list>
                </q-menu>
            </div>

            <div class="q-ml-md cursor-pointer non-selectable  q-pl-sm p-pr-sm">
                Tools
                <q-menu auto-close>
                <q-list dense style="min-width: 100px">
                    <q-item clickable>
                    <q-item-section>Find... (Ctrl + F)</q-item-section>
                    </q-item>
                    <q-item clickable>
                    <q-item-section>Replace... (Ctrl + H)</q-item-section>
                    </q-item>
                    <q-separator />
                    <q-item clickable>
                    <q-item-section>Text Patcher</q-item-section>
                    </q-item>
                </q-list>
                </q-menu>
            </div>
        </q-bar>
        <regex-tool />
        <find-tool />
    </div>
</template>

<script>
import Vue from 'vue'
import Component from 'vue-class-component';
import RegexTool from './RegexTool.vue';
import FindTool from './FindTool.vue';

@Component({
    components:{
        RegexTool,
        FindTool
    }
})
export default class TranslatorMenuBar extends Vue {

}
</script>

<style>

</style>