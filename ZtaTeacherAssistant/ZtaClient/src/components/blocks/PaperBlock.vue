<script lang="ts">
import axios from 'axios'
import { ElNotification } from 'element-plus'
import PaperItem from '../items/PaperItem.vue'
import PaperForm from '../items/PaperForm.vue'

interface iPaperParams {
  pid: number | string | null,
  pname: string | null,
  psource: string | null,
  pyear: string | null,
  ptype: string | null,
  level: string | null
}
interface iData {
  level_options: { value: number | null, label: string }[],
  ptype_options: { value: number | null, label: string }[],
  papers: iPaperParams[]
}

export default {
  components: { PaperItem, PaperForm },
  props: {},
  data() : iData {
    return {
      level_options: [
        { value: 0, label: '任意' },
        { value: 1, label: 'CCF-A' },
        { value: 2, label: 'CCF-B' },
        { value: 3, label: 'CCF-C' },
        { value: 4, label: '中文 CCF-A' },
        { value: 5, label: '中文 CCF-B' },
        { value: 6, label: '无级别' }
      ],
      ptype_options: [
        { value: 0, label: '任意' },
        { value: 1, label: 'full paper' },
        { value: 2, label: 'short paper' },
        { value: 3, label: 'poster paper' },
        { value: 4, label: 'demopaper' }
      ],
      papers: [],
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
        ElNotification.success({message: `论文检索成功`, duration: 1000})
        this.papers = res.data
      }).catch((err: any) => {
        ElNotification.error({message: `论文检索失败，${err.response.data.msg ?? err}`})
      })
    },
    postPaper(paper_params: iPaperParams) {
      const pid = paper_params.pid
      if (pid === null) {
        ElNotification.info({message: '请输入论文编号', duration: 3000})
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
      }).then((res: any) => {
        this.papers = [ JSON.parse(JSON.stringify(paper_params)) ]
        ElNotification.success({message: `论文 ${pid} 登记成功`, duration: 1000})
      }).catch((err: any) => {
        ElNotification.error({message: `论文 ${pid} 登记失败，${err.response.data.msg ?? err}`})
      })
    },
    putPaper(paper_params: iPaperParams) {
      const pid = paper_params.pid
      if (pid === null) {
        ElNotification.info({message: '请输入论文编号', duration: 3000})
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
      }).then((res: any) => {
        ElNotification.success({message: `论文 ${pid} 修改成功`, duration: 1000})
        this.papers = [ JSON.parse(JSON.stringify(paper_params)) ]
      }).catch((err: any) => {
        ElNotification.error({message: `论文 ${pid} 修改失败，${err.response.data.msg ?? err}`})
      })
    },
    deletePaper(index: number) {
      if (!index && index !== 0) {
        ElNotification.info({message: '请选择要删除的论文', duration: 3000})
        return
      }
      let pid = this.papers[index].pid
      this.papers.splice(index, 1)
      // delete /api/paper
      axios.delete('/api/paper', { params: { pid: pid }})
        .then((res: any) => {
          ElNotification.success({message: `论文 ${pid} 删除成功`, duration: 1000})
        })
        .catch((err: any) => {
          ElNotification.error({message: `论文 ${pid} 删除失败，${err.response.data.msg ?? err}`})
        })
    }
  },
  mounted() {}
}

</script>

<template>
  <div class='blocktitle thick'>论文查询</div>
  <div class='blocktext Plaintext'>
    <paper-form @get="getPaper" @post="postPaper" @put="putPaper"></paper-form>
  </div>
  <div class='blocktitle thick'>查询结果</div>
  <div class='blocktext'>
    <ul>
      <paper-item
        v-for="(paper, index) in papers"
        :key="index"
        :pid="paper.pid"
        :pname="paper.pname"
        :psource="paper.psource"
        :pyear="paper.pyear"
        :ptype="ptype_options.find(ptype => ptype.value === paper.ptype)?.label"
        :level="level_options.find(level => level.value === paper.level)?.label"
        @delete="deletePaper(index)" />
    </ul>
  </div>
</template>

<style scoped>
.inputli{width: 99%;border: 0;}
button{width: 5rem;height: 2.5rem;margin:.6rem 0}
</style>
