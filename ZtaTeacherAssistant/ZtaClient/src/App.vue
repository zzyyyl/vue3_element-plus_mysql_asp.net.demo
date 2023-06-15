<script lang="ts">
import PaperBlock from './components/blocks/PaperBlock.vue'
import HitokotoBlock from './components/blocks/HitokotoBlock.vue'

export default {
  name: 'App',
  components: {
    PaperBlock,
    HitokotoBlock
  },
  props: {},
  data() {
    return {
      models: {
        paper: '/paper',
        teacher: '/teacher'
      }
    }
  },
  created() {
    const picTotal = 13
    let randBgIndex = Math.floor(Math.random() * picTotal) % picTotal
    document.body.style.backgroundImage = 'url(backgroundpics/' + randBgIndex + '.jpg)'
    document.body.style.backgroundPosition = 'center center'
    document.body.style.backgroundRepeat = 'no-repeat'
  }
}
</script>

<template>
  <nav class="maintitle">
    <ul>
      <li class="EnglishP thick maintitletext"><a href="/">ZTA</a></li>
      <li v-for="(value, key) in models" :key="key" class="Code">
        <a :href="value">{{ key }}</a>
      </li>
    </ul>
  </nav>
  <div class="transbox">
    <div class="sidebar"><hitokoto-block /></div>
    <div class="mainbar"><paper-block /></div>
  </div>
</template>

<style scoped></style>
