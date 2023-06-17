<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import PaperForm from '../components/items/PaperForm.vue'
const level_options = [
  { value: 1, label: 'CCF-A' },
  { value: 2, label: 'CCF-B' },
  { value: 3, label: 'CCF-C' },
  { value: 4, label: '中文 CCF-A' },
  { value: 5, label: '中文 CCF-B' },
  { value: 6, label: '无级别' }
]
const ptype_options = [
  { value: 1, label: 'full paper' },
  { value: 2, label: 'short paper' },
  { value: 3, label: 'poster paper' },
  { value: 4, label: 'demopaper' }
]
</script>

<script lang="ts">
import axios from 'axios'
import { ElMessage } from 'element-plus'

interface iPaperParams {
  pid: number | string | null,
  pname: string | null,
  psource: string | null,
  pyear: string | null,
  ptype: number | null,
  level: number | null
}

export default {
  data() {
    return {
      papers: [] as iPaperParams[],
    }
  },
  methods: {
    getPaper(paper_params: iPaperParams) {
      // get /api/paper
      axios.get('/api/paper', {
        params: {
          pid: paper_params.pid ?? null,
          pname: paper_params.pname || null,
          psource: paper_params.psource || null,
          pyear: paper_params.pyear || null,
          ptype: paper_params.ptype || null,
          level: paper_params.level || null
        },
      }).then((res: any) => {
        ElMessage.success({showClose: true, message: `论文检索成功`, duration: 1000})
        this.papers = res.data
      }).catch((err: any) => {
        ElMessage.error({showClose: true, message: `论文检索失败，${err.response.data.msg ?? err}`})
      })
    },
    postPaper(paper_params: iPaperParams) {
      const pid = paper_params.pid
      if (pid === null) {
        ElMessage.info({showClose: true, message: '请输入论文编号', duration: 3000})
        return
      }
      // post /api/paper
      axios.post('/api/paper', {
        pid: pid ?? null,
        pname: paper_params.pname || null,
        psource: paper_params.psource || null,
        pyear: paper_params.pyear || null,
        ptype: paper_params.ptype || null,
        level: paper_params.level || null
      }).then(() => {
        this.papers = [ JSON.parse(JSON.stringify(paper_params)) ]
        ElMessage.success({showClose: true, message: `论文 ${pid} 登记成功`, duration: 1000})
      }).catch((err: any) => {
        ElMessage.error({showClose: true, message: `论文 ${pid} 登记失败，${err.response.data.msg ?? err}`})
      })
    },
    putPaper(paper_params: iPaperParams) {
      const pid = paper_params.pid
      if (pid === null) {
        ElMessage.info({showClose: true, message: '请输入论文编号', duration: 3000})
        return
      }
      // put /api/paper
      axios.put('/api/paper', {
        pid: pid ?? null,
        pname: paper_params.pname || null,
        psource: paper_params.psource || null,
        pyear: paper_params.pyear || null,
        ptype: paper_params.ptype || null,
        level: paper_params.level || null
      }).then(() => {
        ElMessage.success({showClose: true, message: `论文 ${pid} 修改成功`, duration: 1000})
        this.papers = [ JSON.parse(JSON.stringify(paper_params)) ]
      }).catch((err: any) => {
        ElMessage.error({showClose: true, message: `论文 ${pid} 修改失败，${err.response.data.msg ?? err}`})
      })
    },
    deletePaper(index: number) {
      if (!index && index !== 0) {
        ElMessage.info({showClose: true, message: '请选择要删除的论文', duration: 3000})
        return
      }
      let pid = this.papers[index].pid
      this.papers.splice(index, 1)
      // delete /api/paper
      axios.delete('/api/paper', { params: { pid: pid }})
        .then(() => {
          ElMessage.success({showClose: true, message: `论文 ${pid} 删除成功`, duration: 1000})
        })
        .catch((err: any) => {
          ElMessage.error({showClose: true, message: `论文 ${pid} 删除失败，${err.response.data.msg ?? err}`})
        })
    }
  }
}

</script>

<template>
  <div class='blocktitle thick'>论文查询</div>
  <div class='blocktext Plaintext'>
    <paper-form
      @get="getPaper"
      @post="postPaper"
      @put="putPaper" />
  </div>
  <div v-if="papers.length" class='blocktitle thick'>查询结果</div>
  <div v-if="papers.length" class='blocktext'>
    <el-table :data="papers" max-height="400" stripe>
      <el-table-column fixed prop="pid" label="编号" width="60"></el-table-column>
      <el-table-column prop="pname" label="名称" width="120"></el-table-column>
      <el-table-column prop="psource" label="来源" width="100"></el-table-column>
      <el-table-column prop="pyear" label="年份" width="120"></el-table-column>
      <el-table-column prop="ptype" label="类型" width="120">
        <template #default="scope">
          {{ ptype_options.find(type => type.value === scope.row.ptype)?.label }}
        </template>
      </el-table-column>
      <el-table-column prop="level" label="级别" width="120">
        <template #default="scope">
          {{ level_options.find(level => level.value === scope.row.level)?.label }}
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <el-button plain type="primary" @click="$router.push(`/paper/${scope.row.pid}`)">详 情</el-button>
          <danger-button label="删 除" message="是否删除该论文？" @commit="deletePaper(scope.$index)"/>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<style scoped>
.inputli{width: 99%;border: 0;}
/* button{width: 5rem;height: 2.5rem;margin:.6rem 0} */
</style>
