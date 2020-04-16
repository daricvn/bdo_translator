<template>
  <q-dialog v-model="open">
      <q-card class="bg-primary text-white">
        <q-bar>
          <div>
              Localization Patcher Tool
          </div>

          <q-space />

          <q-btn dense flat icon="close" v-close-popup>
          </q-btn>
        </q-bar>
        <q-card-section class="q-pt-sm">
          <div class="column">
              <div class="col">
                  <span class="text-caption">
                      Please select a source file, and set destination file to begin patch file. <br/>
                      This tool allows to copy texts changed from source to destination.
                  </span>
              </div>
              <!-- Source  -->
              <div class="col">
                  <div class="row">
                      <div class="col">
                            <q-input label="Source" square outlined dense :value="source" readonly />
                      </div>
                      <div class="col-auto q-pl-sm">
                            <q-btn unelevated color="primary" @click="browseSource">
                                Browse...
                            </q-btn>
                      </div>
                  </div>
              </div>
              <!-- Destination  -->
              <div class="col">
                  <div class="row">
                      <div class="col">
                            <q-input label="Source" square outlined dense :value="destination" readonly />
                      </div>
                      <div class="col-auto q-pl-sm">
                            <q-btn unelevated color="primary" @click="browseDest">
                                Browse...
                            </q-btn>
                      </div>
                  </div>
              </div>
              <!-- Action  -->
              <div class="col text-center q-pt-md" >
                  <q-btn unelevated @click="hide">Close</q-btn>
                  <q-btn class="q-ml-md" unelevated color="primary" @click="submit"
                    :disable="!source || !destination"
                  >Patch</q-btn>
              </div>
          </div>
        </q-card-section>
      </q-card>
  </q-dialog>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { Inject, Watch } from 'vue-property-decorator';
import AppService from '../services/AppService';
import { QVueGlobals } from 'quasar';

@Component
export default class PatcherTool extends Vue {
    open: boolean = false;
    source: string ='';
    destination: string ='';
    @Inject() appService!: AppService;
    $q!: QVueGlobals;

    mounted(){
        const showPatcherTools = this.show;
        (window as any).$app={
            ...(window as any).$app,
            showPatcherTools
        }
    }

    browseSource(){
        this.$q.loading.show();
        this.appService.fileDialog(true, false).then(res=>{
            if (res.data && res.data.Data)
                this.source = res.data.Data;
            else this.source = '';
        }).finally(()=>
        this.$q.loading.hide())
    }
    browseDest(){
        this.$q.loading.show();
        this.appService.fileDialog(true, false).then(res=>{
            if (res.data && res.data.Data)
                this.destination = res.data.Data;
            else this.destination = '';
        }).finally(()=>this.$q.loading.hide())
    }

    show(){
        this.open=true;
    }
    hide(){
        this.open = false;
    }

    submit(){
        this.$q.loading.show();
        this.appService.patchFile(this.source,this.destination).then(res=>{
            this.$q.dialog({
                title:"Patch completed!",
                message: "Do you want to open the destination?",
                cancel: true
            }).onOk(()=>{
                this.appService.openExplorer(this.destination);
            })
        }).finally(()=>{
            this.$q.loading.hide();
        });
    }
    
}
</script>

<style>

</style>