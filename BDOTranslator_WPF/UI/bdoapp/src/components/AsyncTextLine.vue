<template>
  <q-item
    :class="{ 'bg-blue': selected }"
    dense
    :clickable="!selected"
    @click="onClick"
  >
    <q-item-section>
      <q-item-label v-if="!!data && data.text" style="white-space: nowrap; overflow: hidden; max-width: 380px;">{{ data.text }}</q-item-label>
    </q-item-section>
  </q-item>
</template>

<script lang="ts">
import Vue from "vue";
import Component from "vue-class-component";
import { Prop, Inject, Emit, Watch, PropSync } from "vue-property-decorator";
import AppService from "../services/AppService";
import TextLine from '../models/TextLine';

@Component
export default class AsyncTextLine extends Vue {
  @Prop({
    default: 0
  })
  index!: number;

    @Prop() selected: boolean = false;
    @Prop() data: TextLine = new TextLine();

    get displayText(){
        if (this.data && this.data.text)
            return this.data.text;
        return '';
    }
    
  mounted() {
    this.onRequested();
  }

    @Watch("selected")
    onSelected(){
        if (this.selected)
            this.onChange();
    }

    @Emit("click")
  onClick() {
      return this.index;
  }

    @Emit("change")
  onChange(){
      let data= {
          index: this.index,
          text: this.data?.text
      }
      return data;
  }

  @Emit("requested")
  onRequested(){
      return this.index
  }
}
</script>

<style>
</style>