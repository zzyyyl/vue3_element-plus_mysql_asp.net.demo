<script lang="ts">
import PaperForm from './components/PaperForm.vue'
import HitokotoItem from './components/HitokotoItem.vue'

export default {
  name: 'App',
  components: {
    PaperForm,
    HitokotoItem
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
    const picTotal = 13;
    let randBgIndex = Math.floor(Math.random() * picTotal) % picTotal;
    document.body.style.backgroundImage = "url(backgroundpics/" + randBgIndex + ".jpg)";
    document.body.style.backgroundPosition = "center center";
    document.body.style.backgroundRepeat = "no-repeat";
  }
}

</script>

<template>
  <nav class='maintitle'><ul>
    <li class='EnglishP thick maintitletext'><a href='/'>ZTA</a></li>
    <li v-for="(value, key) in models" :key="key" class="Code">
      <a :href="value">{{ key }}</a>
    </li>
	</ul></nav>
  <div class="transbox">
    <div class="sidebar"><hitokoto-item /></div>
    <div class="mainbar"><paper-form /></div>
  </div>
</template>

<style scoped>
</style>
