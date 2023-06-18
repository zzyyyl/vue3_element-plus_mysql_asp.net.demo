<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import NumberInput from '@/components/items/NumberInput.vue'
import SelectOptions from '@/components/items/SelectOptions.vue'

const cnature_options = [
  { value: 1, label: '本科生课程' },
  { value: 2, label: '研究生课程' }
]
const term_options = [
  { value: 1, label: '春季学期' },
  { value: 2, label: '夏季学期' },
  { value: 3, label: '秋季学期' }
]
</script>

<script lang="ts">
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iTeacher {
  tid: string
  cid: string
  tyear: number | null
  tterm: number | null
  thour: number | null
}

interface iCourse {
  cid: string | null
  cname: string | null
  chour: number | null
  cnature: number | null
}

interface iCourseDetail {
  teachers: iTeacher[]
  course: iCourse
}

export default {
  data() {
    return {
      courses: {} as iCourseDetail
    }
  },
  methods: {},
  mounted() {
    const cid = this.$route.params.cid
    if (cid) {
      axios
        .get(`/api/course-taught/${cid}`)
        .then((res: any) => {
          this.courses = res.data
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
  <div class="blocktitle thick">课程详情</div>
  <div class="blocktext Plaintext">
    <el-table :data="[courses.course]">
      <el-table-column fixed prop="cid" label="编号" sortable width="80" />
      <el-table-column prop="cname" label="名称" width="auto" />
      <el-table-column prop="chour" label="学时" sortable width="80" />
      <el-table-column prop="cnature" label="性质" sortable width="120">
        <template #default="{ row }">
          <span>{{
            cnature_options.find((item) => item.value === row?.cnature)?.label
          }}</span>
        </template>
      </el-table-column>
    </el-table>
    <br />
    <el-table :data="courses.teachers" max-height="500">
      <el-table-column prop="tid" label="教师编号" />
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
</style>
