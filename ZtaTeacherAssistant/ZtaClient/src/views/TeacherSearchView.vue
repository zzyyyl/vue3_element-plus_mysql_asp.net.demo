<script setup lang="ts">
import TeacherForm from '../components/TeacherForm.vue'

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

</script>

<script lang="ts">
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iTeacherParams {
  tid: string
  tname: string | null
  gender: number | null
  title: number | null
  orderby: 'tid' | 'tname' | 'gender' | 'title'
  desc: boolean
  limit: number
}

export default {
  data() {
    return {
      teachers: [] as iTeacherParams[],
      title: '教师检索'
    }
  },
  methods: {
    getTeacher(teacher_params: iTeacherParams) {
      // get /api/teacher
      axios
        .get('/api/teacher', {
          params: {
            tid: teacher_params.tid || null,
            tname: teacher_params.tname || null,
            gender: teacher_params.gender || null,
            title: teacher_params.title || null,
            orderby: teacher_params.orderby || null,
            desc: teacher_params.desc || null,
            limit: teacher_params.limit || null
          }
        })
        .then((res: any) => {
          ElMessage.success({ showClose: true, message: `教师检索成功`, duration: 1000 })
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
  <div class="blocktitle thick">教师检索</div>
  <div class="blocktext Plaintext">
    <teacher-form @get="getTeacher" />
  </div>
  <div v-if="teachers.length" class="blocktitle thick">查询结果</div>
  <div v-if="teachers.length" class="blocktext">
    <el-table :data="teachers" max-height="400" stripe>
      <el-table-column prop="tid" label="教师编号" sortable></el-table-column>
      <el-table-column prop="tname" label="姓名" sortable></el-table-column>
      <el-table-column prop="gender" label="性别" sortable width="120">
        <template #default="scope">
          <span>{{
            gender_options.find((gender) => gender.value === scope.row.gender)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="title" label="职称" sortable width="120">
        <template #default="scope">
          <span>{{
            title_options.find((title) => title.value === scope.row.title)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/teacher/${scope.row.tid}`)"
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
/* button{width: 5rem;height: 2.5rem;margin:.6rem 0} */
</style>
