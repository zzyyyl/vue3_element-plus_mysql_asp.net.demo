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
  courseTaught: {
    tid: string
    cid: string
    tyear: number | null
    tterm: number | null
    thour: number | null
  },
  course: {
    cid: string
    cname: string | null
    chour: number | null
    cnature: number | null
  }
}
interface iProject {
  projectUndertaken: {
    jid: string
    tid: string
    jtrank: number | null
    jtbudget: number | null
  },
  project: {
    jid: string
    jname: string | null
    jsource: string | null
    jtype: number | null
    jbudgets: number | null
    styear: number | null
    edyear: number | null
  }
}
interface iPaper {
  publishPaper: {
    tid: string
    pid: number
    ptrank: number | null
    correspond: boolean | null
  },
  paper: {
    pid: number
    pname: string | null
    psource: string | null
    pyear: string | null
    ptype: number | null
    level: number | null
  }
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
      styear: null as null | number,
      edyear: null as null | number,
      teachers: {} as iTeacherDetail
    }
  },
  methods: {
    updateData() {
      const tid = this.$route.params.tid
      if (tid) {
        axios
          .get(`/api/teacher/detail/${tid}`, {
            params: { styear: this.styear, edyear: this.edyear }
          })
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
  },
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
    <el-row justify="space-evenly">
      <el-col span="7">
        <number-input v-model="styear" placeholder="起始年份"></number-input>
      </el-col>
      <el-col span="7">
        <number-input v-model="edyear" placeholder="结束年份"></number-input>
      </el-col>
      <el-col span="7">
        <el-button type="primary" @click="updateData">查询</el-button>
      </el-col>
    </el-row>
    <el-table :data="teachers.courses" max-height="500">
      <el-table-column prop="courseTaught.cid" label="课程编号" :width="80" />
      <el-table-column prop="course.cname" label="课程名称" />
      <el-table-column prop="courseTaught.tyear" label="开课年份" />
      <el-table-column prop="courseTaught.tterm" label="开课学期">
        <template #default="{ row }">
          <span>{{
            term_options.find((item) => item.value === row?.courseTaught.tterm)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="courseTaught.thour" label="课时数" />
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/course/${scope.row.course.cid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="teachers.projects" max-height="500">
      <el-table-column prop="projectUndertaken.jid" label="项目编号" :width="80" />
      <el-table-column prop="project.jname" label="项目名称" />
      <el-table-column prop="project.jsource" label="项目来源" />
      <el-table-column label="项目中排名">
        <template #default="scope">
          <el-tag>第 {{ scope.row.projectUndertaken.jtrank }} 负责人</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="项目经费">
        <template #default="scope">
          <span>{{ scope.row.projectUndertaken.jtbudget }} / {{ scope.row.project.jbudgets }}</span>
        </template>
      </el-table-column>
      <el-table-column label="项目起止年份">
        <template #default="scope">
          <span>{{
            scope.row.project.styear +
            ' - ' +
            scope.row.project.edyear
          }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/project/${scope.row.project.jid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="teachers.papers" max-height="500">
      <el-table-column prop="publishPaper.pid" label="论文编号" :width="80" />
      <el-table-column prop="paper.pname" label="论文名称" />
      <el-table-column prop="paper.psource" label="论文来源" />
      <el-table-column label="论文发表年份">
        <template #default="scope">
          <span>{{ scope.row.paper.pyear }}</span>
        </template>
      </el-table-column>
      <el-table-column label="论文中排名">
        <template #default="scope">
          <el-tag>第 {{ scope.row.publishPaper.ptrank }} 作者</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="是否通讯作者">
        <template #default="scope">
          <el-tag
            v-if="scope.row.publishPaper.correspond"
            >通讯作者</el-tag
          >
          <el-tag v-else type="info"
            >非通讯作者</el-tag
          >
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/paper/${scope.row.paper.pid}`)"
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
