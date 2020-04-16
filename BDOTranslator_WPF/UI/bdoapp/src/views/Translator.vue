<template>
    <div>
        <translator-menu-bar></translator-menu-bar>
        <q-page-container>
            <div class="column">
                <div class="col">
                    <div class="row">
                        <div class="col q-pa-sm">
                            <q-input square outlined dense :value="fileName" readonly />
                        </div>
                        <div class="col-auto q-pt-sm q-pb-sm q-pr-sm">
                            <q-btn unelevated color="primary" @click="browse">
                                <q-icon class="on-left" name="search"></q-icon>
                                Browse...
                            </q-btn>
                        </div>
                    </div>
                </div>
                <div class="col">
                    <div class="row full-height">
                        <div class="col-5">
                            <div class="column full-height">
                                <div class="col">
                                    <q-virtual-scroll separator ref="list-view"
                                        :items-fn="getList"
                                        :items-size="listCount"
                                        style="max-height: 492px"
                                        >
                                        <template v-slot="{ item, index }">
                                            <async-text-line :key="index" :index="item.index" :selected="selectedIndex == item.index" @click="onClick"
                                                :data="lines[item.index] || { text: '' }"
                                                @requested="onUpdateRequested"
                                                @change="onIndexChanged" />
                                        </template>
                                    </q-virtual-scroll>
                                </div>
                                <div class="col-auto q-pl-md">
                                    <div class="row items-center">
                                        <div class="col-auto q-pr-sm">
                                            <q-input dense outlined v-model.number="viewIndex" label="Line" type="number" min="1" :max="listCount+1" :disable="listCount==0" />
                                        </div>
                                        <div class="col">
                                            <q-btn unelevated color="primary" @click="goTo(viewIndex-1)" :disable="listCount==0">Go to</q-btn>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-7">
                            <editor-panel :oldText="oldText" :old-text="oldText" @submit="updateText">
                                <q-checkbox v-model="autoReplace" label="Auto-replace the same value" />
                            </editor-panel>
                        </div>
                    </div>
                </div>
            </div>
        </q-page-container>
    </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Watch, Inject, Ref, ProvideReactive } from 'vue-property-decorator';
import AppService from '../services/AppService';
import TextLine from '../models/TextLine';
import EditorPanel from '../components/EditorPanel.vue';
import { QVueGlobals, QVirtualScroll } from 'quasar';
import AsyncTextLine from '../components/AsyncTextLine.vue';
import TranslatorMenuBar from '../components/TranslatorMenuBar.vue';

@Component({
    components: {
        EditorPanel,
        AsyncTextLine,
        TranslatorMenuBar
    }
})
export default class Translator extends Vue {
    @Inject() appService!: AppService;
    lines: any = {};
    selectedIndex: number = -1;
    viewIndex: number = 1;
    oldText: string='';
    fileName: string ='';
    listCount: number =0;
    $q!: QVueGlobals;
    autoReplace: boolean = false;
    @Ref("list-view") listView!: QVirtualScroll;

    mounted(){
        const setSelectedIndex = 
            this.setSelectedIndex;
        const setLoading = (bool: boolean)=>
            bool?this.$q.loading.show():this.$q.loading.hide();
        const setListCount = (count: number)=>
            this.listCount = count;
        const setFilePath = (text: string)=>{
            this.fileName=text;
            this.$forceUpdate();
        }
        const getFilePath = ()=>{
            return this.fileName;
        }
        const saveFile = this.writeToFile;
        const goTo = this.goTo;
        const setLine = (index: number, text: string)=>{
            if (this.lines[index])
                this.lines[index] = {
                    ...this.lines[index],
                    text
                };
        }
        const getSelectedIndex = ()=>{
            return this.selectedIndex;
        }
        (window as any).$app = {
            ...(window as any).$app,
            setSelectedIndex,
            setLoading,
            setListCount,
            setFilePath,
            getFilePath,
            setLine,
            goTo,
            getSelectedIndex,
            saveFile
        }
    }

    browse(){
        // this.$q.loading.show();
        this.appService.getFile().finally(()=>{
            this.$q.loading.hide();
        });
    }

    updateText(newText: string){
        if (this.selectedIndex>=0 && this.selectedIndex< this.listCount){
            // this.lines[this.selectedIndex].text = newText;
            this.lines[this.selectedIndex]={
                ...this.lines[this.selectedIndex],
                text: newText
            };
            this.appService.updateText(this.selectedIndex, newText, this.autoReplace).then(()=>{
                this.listView.refresh(-1);
            });
            if (this.selectedIndex< this.listCount){
                this.setSelectedIndex(this.selectedIndex+1);
            }
        }
    }

    onClick(index: number){
        this.setSelectedIndex(index);
    }

    @Watch("selectedIndex")
    onSelectIndexChange(){
        this.viewIndex = this.selectedIndex+1;
    }

    setSelectedIndex(index: number){
        this.selectedIndex = index;
    }

    getList(from: number, size: number){
        var items=[];
        for (let i=0; i<size; i++){
            items.push({
                index: i+from
            });
        }

        return Object.freeze(items);
    }

    onIndexChanged({ index, text }:any){
        this.oldText=text;
    }

    @Watch("listCount")
    onListCountChanged(){
        this.listView.refresh(0);
    }

    onUpdateRequested(index: number){
        this.appService.getLine(index).then(res => {
            let item = JSON.parse(res.data) as TextLine;
            this.$set(this.lines, index, item);
        });
    }

    goTo(index: number){
        if (index>=0 && index< this.listCount){
            this.listView.scrollTo(index);
            this.setSelectedIndex(index);
        }
    }
    
    writeToFile(){
        if (this.listCount>0){
            this.$q.loading.show({
                message: 'Saving to file...'
            })
            this.appService.writeToFile(this.fileName).then(res=>{
                this.$q.notify({
                        message: 'Save successfully.',
                        color: 'primary',
                        position: 'top',
                        actions: [
                            { label: 'Dismiss', color: 'white' }
                        ]
                    })
            }).finally(()=>{
                this.$q.loading.hide();
            })
        }
    }
}
</script>

<style>

</style>