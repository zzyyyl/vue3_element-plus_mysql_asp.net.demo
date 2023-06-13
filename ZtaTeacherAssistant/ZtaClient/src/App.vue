<script lang="ts">
import PaperForm from './components/PaperForm.vue'
import Hitokoto from './components/Hitokoto.vue'

export default {
  name: 'App',
  components: {
    PaperForm,
    Hitokoto
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
    <li v-for="(value, key) in models" class="Code">
      <a :href="value">{{ key }}</a>
    </li>
	</ul></nav>
  <div class="transbox">
    <div class="sidebar"><Hitokoto /></div>
    <div class="mainbar"><PaperForm /></div>
  </div>
</template>

<style scoped>
</style>
