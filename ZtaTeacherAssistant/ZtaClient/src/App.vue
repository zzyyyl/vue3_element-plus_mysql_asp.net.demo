<script setup lang="ts">
import HitokotoView from './views/HitokotoView.vue'
import { useRoute } from 'vue-router'
import { watch } from 'vue'

const route = useRoute()

const models = {
  paper: '/search/paper',
  teacher: '/search/teacher'
}

const picTotal = 13
let randBgIndex = Math.floor(Math.random() * picTotal) % picTotal
document.body.style.backgroundImage = 'url(/backgroundpics/' + randBgIndex + '.jpg)'
document.body.style.backgroundPosition = 'center center'
document.body.style.backgroundRepeat = 'no-repeat'
document.title = `ZTA - ${route.path == '/' ? '首页' : route.path.slice(1)}`

watch(
  () => route.path,
  async newParam => {
    document.title = `ZTA - ${newParam == '/' ? '首页' : newParam.slice(1)}`
  }
)
</script>

<template>
  <el-container>
    <el-header>
      <nav class="maintitle">
        <ul>
          <li class="thick">
            <router-link to="/">ZTA</router-link></li>
          <li v-for="(value, key) in models" :key="key" class="Code">
            <router-link :to="value">{{ key }}</router-link>
          </li>
        </ul>
      </nav>
    </el-header>
    <el-main>
      <div class="transbox">
        <el-row justify="space-evenly">
          <el-col :xs="22" :sm="22" :md="14" :lg="15" :xl="16">
            <router-view />
          </el-col>
          <el-col :xs="22" :sm="22" :md="8" :lg="7" :xl="6">
            <hitokoto-view />
          </el-col>
        </el-row>
      </div>
    </el-main>
  </el-container>
</template>

<style scoped>
</style>
