<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import NumberInput from '@/components/items/NumberInput.vue'
import SelectOptions from '@/components/items/SelectOptions.vue'

const term_options = [
  { value: 1, label: '春季学期' },
  { value: 2, label: '夏季学期' },
  { value: 3, label: '秋季学期' }
]

const gender_options = [
  { value: 1, label: '男' },
  { value: 2, label: '女' }
]

const title_options = [
  { value: 1, label: '博士后' },
  { value: 2, label: '助教' },
  { value: 3, label: '讲师' },
  { value: 4, label: '副教授' },
  { value: 5, label: '特任教授' },
  { value: 6, label: '教授' },
  { value: 7, label: '助理研究员' },
  { value: 8, label: '特任副研究员' },
  { value: 9, label: '副研究员' },
  { value: 10, label: '特任研究员' },
  { value: 11, label: '研究员' }
]

const orderby_options = [
  { value: 'tid', label: '教师编号' },
  { value: 'tname', label: '教师姓名' },
  { value: 'gender', label: '教师性别' },
  { value: 'title', label: '教师职称' },
]
</script>

<script lang="ts">
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iTeacher {
  tid: string
  tname: string | null
  gender: number | null
  title: number | null
}
interface iCourse {
  tid: string
  cid: string
  tyear: number | null
  tterm: number | null
  thour: number | null
}
interface iProject {
  jid: string
  tid: string
  jtrank: number | null
  jtbudget: number | null
}
interface iPaper {
  pid: number
  tid: string
  ptrank: number | null
  correspond: boolean | null
}

interface iTeacherDetail {
  teacher: iTeacher
  courses: iCourse[]
  projects: iProject[]
  papers: iPaper[]
}

export default {
  data() {
    return {
      teachers: {} as iTeacherDetail
    }
  },
  methods: {},
  mounted() {
    const tid = this.$route.params.tid
    if (tid) {
      axios
        .get(`/api/teacher/detail/${tid}`)
        .then((res: any) => {
          this.teachers = res.data
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `教师检索失败，${err.response.data.msg ?? err}`
          })
        })
    }
  }
}
</script>

<template>
  <div class="blocktitle thick">教师详情</div>
  <div class="blocktext Plaintext">
    <el-table :data="[teachers.teacher]">
      <el-table-column prop="tid" label="教师编号" sortable></el-table-column>
      <el-table-column prop="tname" label="姓名" sortable></el-table-column>
      <el-table-column prop="gender" label="性别" sortable width="120">
        <template #default="scope">
          <span>{{
            gender_options.find((gender) => gender.value === scope.row?.gender)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="title" label="职称" sortable width="120">
        <template #default="scope">
          <span>{{
            title_options.find((title) => title.value === scope.row?.title)?.label
          }}</span>
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="teachers.courses" max-height="500">
      <el-table-column prop="cid" label="课程编号" />
      <el-table-column prop="tyear" label="开课年份" />
      <el-table-column prop="tterm" label="开课学期">
        <template #default="{ row }">
          <span>{{
            term_options.find((item) => item.value === row?.tterm)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="thour" label="课时数" />
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/course/${scope.row.cid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="teachers.projects" max-height="500">
      <el-table-column prop="jid" label="项目编号" />
      <el-table-column label="项目中排名">
        <template #default="scope">
          <el-tag>第 {{ scope.row.jtrank }} 负责人</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="jtbudget" label="项目经费" />
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/project/${scope.row.jid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="teachers.papers" max-height="500">
      <el-table-column prop="pid" label="论文编号" />
      <el-table-column label="论文中排名">
        <template #default="scope">
          <el-tag>第 {{ scope.row.ptrank }} 作者</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="是否通讯作者">
        <template #default="scope">
          <el-tag
            v-if="scope.row.correspond"
            >通讯作者</el-tag
          >
          <el-tag v-else type="info"
            >非通讯作者</el-tag
          >
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/paper/${scope.row.pid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<style scoped>
.inputli {
  width: 99%;
  border: 0;
}
</style>
