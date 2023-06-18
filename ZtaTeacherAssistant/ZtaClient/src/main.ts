import './assets/main.css'
import 'element-plus/dist/index.css'

import { createApp } from 'vue'
import App from './App.vue'
import ElementPlus from 'element-plus'
import { createRouter, createWebHashHistory } from 'vue-router'

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/',
      component: () => import('./views/MainView.vue')
    },
    {
      path: '/search/paper',
      component: () => import('./views/PaperSearchView.vue')
    },
    {
      path: '/paper/:pid',
      component: () => import('./views/PaperDetailView.vue')
    },
    {
      path: '/search/project',
      component: () => import('./views/ProjectSearchView.vue')
    },
    {
      path: '/project/:jid',
      component: () => import('./views/ProjectDetailView.vue')
    },
    {
      path: '/search/course',
      component: () => import('./views/CourseSearchView.vue')
    },
    {
      path: '/course/:cid',
      component: () => import('./views/CourseDetailView.vue')
    },
    {
      path: '/search/teacher',
      component: () => import('./views/TeacherSearchView.vue')
    },
    {
      path: '/teacher/:tid',
      component: () => import('./views/TeacherDetailView.vue')
    },
  ]
})

const app = createApp(App)

app.use(router).use(ElementPlus).mount('#app')
