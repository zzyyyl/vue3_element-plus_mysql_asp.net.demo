<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import NumberInput from '@/components/items/NumberInput.vue'
import StringInput from '@/components/items/StringInput.vue'
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
  cid: string
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
      courses: {} as iCourseDetail,
      origin_courses: {} as iCourseDetail,
      course_editable: false,
    }
  },
  methods: {
    onAddTeacher() {
      this.courses.teachers.push({
        tid: '',
        cid: this.courses.course.cid,
        tyear: null,
        tterm: null,
        thour: null
      })
    },
    onCourseReset() {
      this.course_editable = false
      this.courses = JSON.parse(JSON.stringify(this.origin_courses))
    },
    onCourseSubmit() {
      const cid = this.$route.params.cid
      let thour_sum = 0
      for (const teacher of this.courses.teachers) {
        thour_sum += teacher.thour ?? 0
      }
      if (this.courses.course.chour != thour_sum) {
        ElMessage.error({
          showClose: true,
          message: `课程修改失败，课程学时与教师学时总和不相等`
        })
        return
      }
      if (!cid) {
        ElMessage.error({
          showClose: true,
          message: `课程修改失败，课程编号不能为空`
        })
        return
      }
      axios
        .post(`/api/course-taught/${cid}`, this.courses.teachers)
        .then((res: any) => {
          this.course_editable = false
          ElMessage.success({
            showClose: true,
            message: `课程修改成功`
          })
          this.origin_courses = JSON.parse(JSON.stringify(this.courses))
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `课程修改失败，${err.response.data.msg ?? err}`
          })
        })
    }
  },
  mounted() {
    const cid = this.$route.params.cid
    if (cid) {
      axios
        .get(`/api/course-taught/${cid}`)
        .then((res: any) => {
          this.courses = res.data
          this.origin_courses = JSON.parse(JSON.stringify(this.courses))
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
      <el-table-column label="教师编号">
        <template #default="scope">
          <string-input
            v-if="course_editable"
            v-model="scope.row.tid"
            placeholder="请输入教师编号"
            style="width: 100%"
          />
          <span v-else>{{ scope.row.tid }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="tyear" label="开课年份">
        <template #default="{ row }">
          <number-input
            v-if="course_editable"
            v-model="row.tyear"
            placeholder="请输入开课年份"
            style="width: 100%"
          />
          <span v-else>{{ row.tyear }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="tterm" label="开课学期">
        <template #default="{ row }">
          <select-options
            v-if="course_editable"
            v-model="row.tterm"
            :options="term_options"
            placeholder="请选择开课学期"
            style="width: 100%"
          />
          <span v-else>{{
            term_options.find((item) => item.value === row?.tterm)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="thour" label="课时数">
        <template #default="{ row }">
          <number-input
            v-if="course_editable"
            v-model="row.thour"
            placeholder="请输入课时数"
            style="width: 100%"
          />
          <span v-else>{{ row.thour }}</span>
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
    <el-row justify="space-between">
      <el-col v-if="!course_editable">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @click="course_editable = true"
          >启用编辑</el-button
        >
      </el-col>
      <el-col :span="7" v-if="course_editable">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @click="onAddTeacher"
          >增加任课教师</el-button
        >
      </el-col>
      <el-col :span="7" v-if="course_editable">
        <danger-button
          label="放弃更改"
          message="是否放弃更改？"
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @commit="onCourseReset"
        />
      </el-col>
      <el-col :span="7" v-if="course_editable">
        <danger-button
          label="确认更改"
          message="是否确认更改？"
          type="primary"
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @commit="onCourseSubmit"
        />
      </el-col>
    </el-row>
  </div>
</template>

<style scoped>
.inputli {
  width: 99%;
  border: 0;
}
</style>
