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
  <el-container>
    <el-header>
      <!-- <nav class="maintitle">
        <ul>
          <li class="thick"><a href="/">ZTA</a></li>
          <li v-for="(value, key) in models" :key="key" class="Code">
            <a :href="value">{{ key }}</a>
          </li>
        </ul>
      </nav> -->
    </el-header>
    <el-main>
      <div class="transbox">
        <el-row justify="space-evenly">
          <el-col :xs="22" :sm="22" :md="14" :lg="15" :xl="16">
            <paper-block />
          </el-col>
          <el-col :xs="22" :sm="22" :md="8" :lg="7" :xl="6">
            <hitokoto-block />
          </el-col>
        </el-row>
      </div>
    </el-main>
  </el-container>
</template>

<style scoped>
</style>
