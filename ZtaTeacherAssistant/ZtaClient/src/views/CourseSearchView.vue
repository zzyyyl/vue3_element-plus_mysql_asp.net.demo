<script setup lang="ts">
import CourseForm from '../components/CourseForm.vue'

const cnature_options = [
  { value: 1, label: '本科生课程' },
  { value: 2, label: '研究生课程' }
]

</script>

<script lang="ts">
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iCourseParams {
  cid: string
  cname: string | null
  chour: number | null
  cnature: number | null
  orderby: 'cid' | 'cname' | 'chour' | 'cnature'
  desc: boolean
  limit: number
}

export default {
  data() {
    return {
      projects: [] as iCourseParams[],
      title: '课程检索'
    }
  },
  methods: {
    getCourse(course_params: iCourseParams) {
      // get /api/course
      axios
        .get('/api/course', {
          params: {
            cid: course_params.cid || null,
            cname: course_params.cname || null,
            chour: course_params.chour || null,
            cnature: course_params.cnature || null,
            orderby: course_params.orderby || null,
            desc: course_params.desc || null,
            limit: course_params.limit || null
          }
        })
        .then((res: any) => {
          ElMessage.success({ showClose: true, message: `课程检索成功`, duration: 1000 })
          this.projects = res.data
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `课程检索失败，${err.response.data.msg ?? err}`
          })
        })
    }
  }
}
</script>

<template>
  <div class="blocktitle thick">课程检索</div>
  <div class="blocktext Plaintext">
    <course-form @get="getCourse" />
  </div>
  <div v-if="projects.length" class="blocktitle thick">查询结果</div>
  <div v-if="projects.length" class="blocktext">
    <el-table :data="projects" max-height="400" stripe>
      <el-table-column fixed prop="cid" label="编号" sortable width="80" />
      <el-table-column prop="cname" label="名称" width="auto" />
      <el-table-column prop="chour" label="学时" sortable width="80" />
      <el-table-column prop="cnature" label="性质" sortable width="120">
        <template #default="scope">
          {{ cnature_options.find((nature) => nature.value === scope.row.cnature)?.label }}
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/course/${scope.row.cid}`)"
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
