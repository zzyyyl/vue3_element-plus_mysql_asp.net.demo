<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import ProjectForm from '../components/ProjectForm.vue'
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
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iProjectParams {
  jid: string | null
  jname: string | null
  jsource: string | null
  jtype: number | null
  jbudgets: number | null
  styear: number | null
  edyear: number | null
  orderby: 'jid' | 'jname' | 'jsource' | 'jtype' | 'jbudgets' | 'styear' | 'edyear'
  desc: boolean
  limit: number
}

export default {
  data() {
    return {
      projects: [] as iProjectParams[],
      title: '项目检索'
    }
  },
  methods: {
    getProject(paper_params: iProjectParams) {
      // get /api/project
      axios
        .get('/api/project', {
          params: {
            jid: paper_params.jid || null,
            jname: paper_params.jname || null,
            jsource: paper_params.jsource || null,
            jtype: paper_params.jtype || null,
            jbudgets: paper_params.jbudgets || null,
            styear: paper_params.styear || null,
            edyear: paper_params.edyear || null,
            orderby: paper_params.orderby || null,
            desc: paper_params.desc || null,
            limit: paper_params.limit || null
          }
        })
        .then((res: any) => {
          ElMessage.success({ showClose: true, message: `项目检索成功`, duration: 1000 })
          this.projects = res.data
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `项目检索失败，${err.response.data.msg ?? err}`
          })
        })
    },
    postProject(paper_params: iProjectParams) {
      const jid = paper_params.jid
      if (jid === null) {
        ElMessage.info({ showClose: true, message: '请输入项目编号', duration: 3000 })
        return
      }
      // post /api/project
      axios
        .post('/api/project', {
          jid: jid ?? null,
          jname: paper_params.jname || null,
          jsource: paper_params.jsource || null,
          jtype: paper_params.jtype || null,
          jbudgets: paper_params.jbudgets || null,
          styear: paper_params.styear || null,
          edyear: paper_params.edyear || null
        })
        .then(() => {
          this.projects = [JSON.parse(JSON.stringify(paper_params))]
          ElMessage.success({ showClose: true, message: `项目 ${jid} 登记成功`, duration: 1000 })
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `项目 ${jid} 登记失败，${err.response.data.msg ?? err}`
          })
        })
    },
    deleteProject(index: number) {
      if (!index && index !== 0) {
        ElMessage.info({ showClose: true, message: '请选择要删除的项目', duration: 3000 })
        return
      }
      let jid = this.projects[index].jid
      this.projects.splice(index, 1)
      // delete /api/project
      axios
        .delete('/api/project', { params: { jid: jid } })
        .then(() => {
          ElMessage.success({ showClose: true, message: `项目 ${jid} 删除成功`, duration: 1000 })
        })
        .catch((err: any) => {
          ElMessage.error({
            showClose: true,
            message: `项目 ${jid} 删除失败，${err.response.data.msg ?? err}`
          })
        })
    },
    onMethodChange(method: string) {
      this.title = method === 'get' ? '项目检索' : '项目登记'
    }
  }
}
</script>

<template>
  <div class="blocktitle thick">
    {{ title }}
  </div>
  <div class="blocktext Plaintext">
    <project-form @get="getProject" @post="postProject" @method-change="onMethodChange" />
  </div>
  <div v-if="projects.length" class="blocktitle thick">查询结果</div>
  <div v-if="projects.length" class="blocktext">
    <el-table :data="projects" max-height="400" stripe>
      <el-table-column fixed prop="jid" label="编号" sortable width="80"></el-table-column>
      <el-table-column prop="jname" label="名称" width="120"></el-table-column>
      <el-table-column prop="jsource" label="来源" width="100"></el-table-column>
      <el-table-column prop="jtype" label="类型" sortable width="120">
        <template #default="scope">
          {{ jtype_options.find((type) => type.value === scope.row.jtype)?.label }}
        </template>
      </el-table-column>
      <el-table-column prop="jbudgets" label="经费" sortable width="120"></el-table-column>
      <el-table-column prop="styear" label="起始年份" sortable width="120"></el-table-column>
      <el-table-column prop="edyear" label="结束年份" sortable width="120"></el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/project/${scope.row.jid}`)"
            >详 情</el-button
          >
          <danger-button
            label="删 除"
            message="是否删除该项目？"
            @commit="deleteProject(scope.$index)"
          />
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
