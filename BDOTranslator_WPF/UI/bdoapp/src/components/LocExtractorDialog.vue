<template>
  <q-dialog v-model="open">
      <q-card class="bg-primary text-white">
        <q-bar>
          <div>
              Loc Extractor
          </div>

          <q-space />

          <q-btn dense flat icon="close" v-close-popup>
          </q-btn>
        </q-bar>
        <q-card-section class="q-pt-sm">
          <div class="column">
              <div class="col">
                  <span class="text-caption">
                      Please select a source file, and set destination save file to begin
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
                            <q-input label="Destination" square outlined dense :value="destination" readonly />
                      </div>
                      <div class="col-auto q-pl-sm">
                            <q-btn unelevated color="primary" @click="browseDest">
                                Browse...
                            </q-btn>
                      </div>
                  </div>
              </div>
              <!-- Checkbox  -->
              <div class="col q-pb-sm">
                  <q-checkbox label="Encrypt" v-model="encrypt" color="info">
                  </q-checkbox>
              </div>
              <!-- Action  -->
              <div class="col text-center q-pt-sm" >
                  <q-btn unelevated @click="hide">Close</q-btn>
                  <q-btn class="q-ml-md" unelevated color="positive" @click="submit"
                    :disable="!source || !destination"
                  >{{ encrypt ? "Encrypt":"Extract" }}</q-btn>
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
export default class LocExtractorDialog extends Vue {
    open: boolean = false;
    source: string ='';
    destination: string ='';
    encrypt: boolean = false;
    @Inject() appService!: AppService;
    $q!: QVueGlobals;

    mounted(){
        const showExtractorTools = this.show;
        (window as any).$app={
            ...(window as any).$app,
            showExtractorTools
        }
    }

    @Watch("ecrypt")
    clear(){
        this.source='';
        this.destination='';
    }

    browseSource(){
        this.$q.loading.show();
        this.appService.fileDialog(this.encrypt, !this.encrypt).then(res=>{
            if (res.data && res.data.Data)
                this.source = res.data.Data;
            else this.source = '';
        }).finally(()=>
        this.$q.loading.hide())
    }
    browseDest(){
        this.$q.loading.show();
        this.appService.fileSaveDialog(!this.encrypt, this.encrypt).then(res=>{
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
        this.appService.extractFile(this.source,this.destination, this.encrypt).then(res=>{
            this.$q.dialog({
                title:"Process completed!",
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