<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import NumberInput from '@/components/items/NumberInput.vue'
import StringInput from '@/components/items/StringInput.vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import SelectOptions from '@/components/items/SelectOptions.vue'

const jtype_options = [
  { value: 1, label: '国家级项目' },
  { value: 2, label: '省部级项目' },
  { value: 3, label: '市厅级项目' },
  { value: 4, label: '企业合作项目' },
  { value: 5, label: '其它类型项目' }
]
const orderby_options = [
  { value: 'jid', label: '项目编号' },
  { value: 'jname', label: '项目名称' },
  { value: 'jsource', label: '项目来源' },
  { value: 'jtype', label: '项目类型' },
  { value: 'jbudgets', label: '项目经费' },
  { value: 'styear', label: '起始年份' },
  { value: 'edyear', label: '结束年份' }
]
</script>

<script lang="ts">

interface iManager {
  jid: string
  tid: string
  jtrank: number | null
  jtbudget: number | null
}

interface iProject {
  jid: string
  jname: string | null
  jsource: string | null
  jtype: number | null
  jbudgets: number | null
  styear: number | null
  edyear: number | null
}

interface iProjectDetail {
    project: iProject
    managers: iManager[]
}

export default {
  data() {
    return {
        projects: {} as iProjectDetail,
        origin_projects: {} as iProjectDetail,
        project_editable: false
    }
  },
  methods: {
    sortManager(index: number = -1) {
      if (index >= 0) {
        let jtrank = this.projects.managers[index].jtrank || 0
        let aindex = jtrank > 0 ? jtrank - 1 : 0
        if (this.projects.managers.length > aindex) {
          this.projects.managers[aindex].jtrank = index + 1
        }
      }
      this.projects.managers.sort((a: iManager, b: iManager) => (a.jtrank ?? 0) - (b.jtrank ?? 0))
      this.projects.managers.forEach((manager: iManager, index: number) => {
        manager.jtrank = index + 1
      })
    },
    onAddManager() {
      this.projects.managers.push({
        jid: this.projects.project.jid,
        tid: '',
        jtrank: this.projects.managers.length + 1,
        jtbudget: 0
      })
    },
    removeManager(index: number) {
      this.projects.managers.splice(index, 1)
      this.sortManager()
    },
    handleBudgets() {
      let budgets = 0
      this.projects.managers.forEach((manager: iManager) => {
        budgets += manager.jtbudget ?? 0
      })
      this.projects.project.jbudgets = budgets
    },
    //放弃更改
    onProjectReset() {
      this.project_editable = false
      this.projects = JSON.parse(JSON.stringify(this.origin_projects))
    },
    //提交更改
    onProjectSubmit() {
      const jid = this.$route.params.jid
      axios
        .post(`/api/project/detail/${jid}`, this.projects)
        .then((res: any) => {
          this.project_editable = false
          ElMessage.success({ showClose: true, message: '项目信息更新成功' })
          let key: keyof iProject 
          for (key in this.projects.project) {
            let old_val = this.origin_projects.project[key]
            if (old_val !== null && !this.projects.project[key]) {
              (<typeof old_val>this.projects.project[key]) = old_val
            }
          }
          this.origin_projects = JSON.parse(JSON.stringify(this.projects))
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `项目信息更新失败，${err.response.data.msg ?? err}`
          })
        })
    }
  },
  mounted() {
    const jid = this.$route.params.jid
    if (jid) {
      axios
        .get(`/api/project-undertaken/${jid}`)
        .then((res: any) => {
          this.projects = res.data
          this.projects.managers.sort((a: iManager, b: iManager) => (a.jtrank ?? 0) - (b.jtrank ?? 0))
          this.projects.managers.forEach((manager: iManager, index: number) => {
            manager.jtrank = index + 1
          })
          this.origin_projects = JSON.parse(JSON.stringify(this.projects))
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `项目检索失败，${err.response.data.msg ?? err}`
          })
        })
    }
  },
  computed: {
    date_picker_styear: {
      get() {
        return this.projects.project.styear?.toString()
      },
      set(val: string) {
        this.projects.project.styear = parseInt(val)
        if (this.projects.project.edyear && this.projects.project.styear > this.projects.project.edyear) {
          this.projects.project.edyear = null
        }
      }
    },
    date_picker_edyear: {
      get() {
        return this.projects.project.edyear?.toString()
      },
      set(val: string) {
        this.projects.project.edyear = parseInt(val)
      }
    }
  }
}
</script>

<template>
  <div class="blocktitle thick">项目详情</div>
  <div class="blocktext Plaintext">
    <el-table :data="[projects.project]">
      <el-table-column fixed prop="jid" label="编号" width="60" />
      <el-table-column prop="jname" label="名称" width="auto">
        <template #default="scope">
          <string-input v-if="project_editable" v-model="projects.project.jname" />
          <span v-else>{{ scope.row?.jname }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="jsource" label="来源" width="100">
        <template #default="scope">
          <string-input v-if="project_editable" v-model="projects.project.jsource" />
          <span v-else>{{ scope.row?.jsource }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="jtype" label="类型" :width="project_editable ? 150 : 120">
        <template #default="scope">
          <select-options
            v-if="project_editable"
            v-model="projects.project.jtype"
            :options="jtype_options" />
          <span v-else>{{
            jtype_options.find((option) => option.value === scope.row?.jtype)?.label
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="jbudgets" label="经费" width="100" />
      <el-table-column prop="styear" label="开始年份" :width="project_editable ? 120 : 80">
        <template #default="scope">
          <el-date-picker
            v-if="project_editable"
            v-model="date_picker_styear"
            type="year"
            value-format="YYYY"
            placeholder="选择年份"
            style="width: 100%" />
          <!-- <el-input v-if="project_editable" v-model="projects.project.styear"></el-input> -->
          <span v-else>{{ scope.row?.styear }}</span>
        </template>
      </el-table-column>  
      <el-table-column prop="edyear" label="结束年份" :width="project_editable ? 120 : 80">
        <template #default="scope">
          <el-date-picker
            v-if="project_editable"
            v-model="date_picker_edyear"
            type="year"
            value-format="YYYY"
            placeholder="选择年份"
            :disabled-date="(time: Date) => time.getFullYear() < (projects.project.styear ?? 0)"
            style="width: 100%" />
          <!-- <el-input v-if="project_editable" v-model="projects.project.edyear"></el-input> -->
          <span v-else>{{ scope.row?.edyear }}</span>
        </template>
      </el-table-column>
    </el-table>
    <el-table :data="projects.managers" max-height="500">
      <el-table-column label="负责人排名">
        <template #default="scope">
          <number-input
            v-if="project_editable"
            class="inputli"
            v-model="scope.row.jtrank"
            placeholder="请输入负责人排名"
            @blur="sortManager(scope.$index)"
          />
          <el-tag v-else>第 {{ scope.row.jtrank }} 负责人</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="tid" label="负责人编号">
        <template #default="{ row }">
          <string-input
            v-if="project_editable"
            class="inputli"
            v-model="row.tid"
            placeholder="请输入负责人编号"
          />
          <label v-else> {{ row.tid }} </label>
        </template>
      </el-table-column>
      <el-table-column prop="jtbudget" label="预算">
        <template #default="{ row }">
          <number-input
            v-if="project_editable"
            v-model="row.jtbudget"
            placeholder="请输入预算"
            @blur="handleBudgets"
          />
          <label v-else> {{ row.jtbudget }} </label>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <danger-button
            v-if="project_editable"
            label="删 除"
            message="是否删除该负责人？"
            @commit="removeManager(scope.$index)"
          />
          <el-button v-else plain type="primary" @click="$router.push(`/teacher/${scope.row.tid}`)"
            >详 情</el-button
          >
        </template>
      </el-table-column>
    </el-table>
    <el-row justify="space-between">
      <el-col v-if="!project_editable">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @click="project_editable = true"
          >启用编辑</el-button
        >
      </el-col>
      <el-col :span="7" v-if="project_editable">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @click="onAddManager"
          >增加负责人</el-button
        >
      </el-col>
      <el-col :span="7" v-if="project_editable">
        <danger-button
          label="放弃更改"
          message="是否放弃更改？"
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @commit="onProjectReset"
        />
      </el-col>
      <el-col :span="7" v-if="project_editable">
        <danger-button
          label="确认更改"
          message="是否确认更改？"
          type="primary"
          style="width: 100%; height: 2.5rem; margin: 1.5rem 0 0.5rem 0"
          @commit="onProjectSubmit"
        />
      </el-col>
    </el-row>
    <!-- {{ projects }} -->
  </div>
</template>

<style scoped>
.inputli {
  width: 99%;
  border: 0;
}
</style>
