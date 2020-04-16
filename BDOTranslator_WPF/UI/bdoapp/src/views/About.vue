<template>
    <q-dialog v-model="open" persistent transition-show="flip-down" transition-hide="flip-up">
        <q-card class="bg-warning text-white">
            <q-bar>
                <div>
                    About
                </div>

                <q-space />

                <q-btn dense flat icon="close" v-close-popup>
                </q-btn>
            </q-bar>
            <q-card-section class="q-pt-sm items-center">
                <div class="row">
                    <div class="col-auto">
                        <q-avatar size="160px">
                            <img :src="src">
                        </q-avatar>
                    </div>
                    <div class="col q-pl-md">
                        BDO Translator is a tool helping translate Black Desert Online other language that Pearl Abyss yet to support. <br />
                        This application is developed by Darick (Ryu™). Also contains an open source software made by AMGarkin on Github. <br />
                        Thanks Pearl Abyss that made a wonderful world like Black Desert.
                        <br />
                        <br />
                        Credits: AMGarkin for BDO encrypt/decrypt tools.
                        <br />
                        Made with <b>Quasar/Vue/Chromely</b>
                    </div>
                </div>
            </q-card-section>
        </q-card>
    </q-dialog>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { LOGO } from '../assets/bdo.logo';

@Component
export default class About extends Vue {
    src: string = LOGO.data;
    open: boolean = false;

    show(){
        this.open = true;
    }
    hide(){
        this.open = false;
    }
}
</script>

<style>

</style>