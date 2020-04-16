<template>
  <q-layout view="lHh Lpr lFf">
    <router-view></router-view>
  </q-layout>
</template>

<script>
import Vue from "vue";
import Component from "vue-class-component";
import AppService from "./services/AppService";
import { Provide } from "vue-property-decorator";

@Component()
export default class App extends Vue {
  @Provide() appService = new AppService();

  mounted() {
    document.addEventListener(
      "keypress",
      ev => {
        if (this.$q.loading.isActive) return;
        if (ev.ctrlKey && ev.code == "KeyD") {
          window.$app.duplicate();
        }
        if (ev.ctrlKey && ev.code == "KeyH") {
          if (window.$app.isFindToolsOpen())
            window.$app.hideFindTools();
          window.$app.showRegexTools();
        }
        if (ev.ctrlKey && ev.code == "KeyF") {
          if (window.$app.isRegexToolsOpen())
            window.$app.hideRegexTools();
          window.$app.showFindTools();
        }
        if (ev.ctrlKey && ev.code =='KeyS'){
          window.$app.saveFile();
        }
      },
      true
    );
  }
}
</script>

<style></style>
