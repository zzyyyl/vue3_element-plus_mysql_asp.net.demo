<script lang="ts">
import axios from 'axios'
import PaperItem from './items/PaperItem.vue'

interface ipaper_params {
  pid: number | null,
  pname: string | null,
  psource: string | null,
  pyear: string | null,
  ptype: string | null,
  level: string | null
}
interface iData {
  submit: CallableFunction | null,
  paper_params: ipaper_params,
  papers: ipaper_params[]
}

export default {
  components: {
    PaperItem
},
  props: {},
  data() : iData {
    return {
      submit: null,
      paper_params: {
        pid: null,
        pname: null,
        psource: null,
        pyear: null,
        ptype: null,
        level: null
      },
      papers: [],
    }
  },
  methods: {
    getPaper() {
      console.log(this.paper_params)
      // get /api/paper
      axios.get('/api/paper', {
        params: {
          pid: this.paper_params.pid ?? null,
          pname: this.paper_params.pname || null,
          psource: this.paper_params.psource || null,
          pyear: this.paper_params.pyear || null,
          ptype: this.paper_params.ptype || null,
          level: this.paper_params.level || null
        },
      }).then((res: any) => {
        console.log(res.data)
        this.papers = res.data
      }).catch((err: any) => {
        console.log(err)
      })
    },
    postPaper() {
      if (this.paper_params.pid === null) {
        alert('请输入论文编号')
        return
      }
      // post /api/paper
      axios.post('/api/paper', {
        pid: this.paper_params.pid ?? null,
        pname: this.paper_params.pname || null,
        psource: this.paper_params.psource || null,
        pyear: this.paper_params.pyear || null,
        ptype: this.paper_params.ptype || null,
        level: this.paper_params.level || null
      }).then((res: any) => {
        console.log(res.data)
        this.papers = [ JSON.parse(JSON.stringify(this.paper_params)) ]
      }).catch((err: any) => {
        console.log(err)
        alert(err.response.data.msg ? err.response.data.msg : err)
      })
    },
    putPaper() {
      if (this.paper_params.pid === null) {
        alert('请输入论文编号')
        return
      }
      // put /api/paper
      axios.put('/api/paper', {
        pid: this.paper_params.pid ?? null,
        pname: this.paper_params.pname || null,
        psource: this.paper_params.psource || null,
        pyear: this.paper_params.pyear || null,
        ptype: this.paper_params.ptype || null,
        level: this.paper_params.level || null
      }).then((res: any) => {
        console.log(res.data)
        this.papers = [ JSON.parse(JSON.stringify(this.paper_params)) ]
      }).catch((err: any) => {
        console.log(err)
        alert(err.response.data.msg ? err.response.data.msg : err)
      })
    },
    removePaper(index: number) {
      let pid = this.papers[index].pid
      this.papers.splice(index, 1)
      // delete /api/paper
      axios.delete('/api/paper', {
        params: {
          pid: pid
        },
      }).then((res: any) => {
        console.log(res.data)
      }).catch((err: any) => {
        console.log(err)
        alert(err.response.data.msg ? err.response.data.msg : err)
      })
    },
    submitForm() {
      this.submit ? this.submit() : null
      this.submit = null
    }
  }
}

</script>

<template>
  <div class='blocktitle thick'>论文查询</div>
  <div class='blocktext Plaintext'>
    <form v-on:submit.prevent="submitForm">
      <div style="float: right; width: 20%; text-align: center;">
        <button @click="submit = getPaper" style="margin: .4rem">&nbsp;检索&nbsp;</button><br/>
        <button @click="submit = postPaper" style="margin: .4rem">&nbsp;登记&nbsp;</button><br/>
        <button @click="submit = putPaper" style="margin: .4rem">&nbsp;修改&nbsp;</button>
      </div>
      <div style="width: 80%; text-align: center;">
        <label for="pid">论文编号</label>&nbsp;
        <!--限定非负整数-->
        <el-input-number
          class="inputli"
          :min=0
          :max=2147483647
          v-model="paper_params.pid"
          id="pid"
          placeholder="pid" />&nbsp;
        <label for="ptype">论文类型</label>&nbsp;
        <select
          class="inputli"
          v-model="paper_params.ptype"
          id="ptype"
          placeholder="ptype" >
          <option :value="null"></option>
          <option value="1">full paper</option>
          <option value="2">short paper</option>
          <option value="3">poster paper</option>
          <option value="4">demopaper</option>
        </select><br/><br/>
        <label for="pname">论文名称</label>&nbsp;
        <input
          class="inputli"
          v-model="paper_params.pname"
          id="pname"
          placeholder="pname" />&nbsp;
        <label for="level">论文级别</label>&nbsp;
        <select
          class="inputli"
          v-model="paper_params.level"
          id="level"
          placeholder="level" >
          <option :value="null"></option>
          <option value="1">CCF-A</option>
          <option value="2">CCF-B</option>
          <option value="3">CCF-C</option>
          <option value="4">中文 CCF-A</option>
          <option value="5">中文 CCF-B</option>
          <option value="6">无级别</option>
        </select><br/><br/>
        <label for="psource">论文来源</label>&nbsp;
        <input
          class="inputli"
          v-model="paper_params.psource"
          id="psource"
          placeholder="psource" />&nbsp;
        <label for="pyear">论文年份</label>&nbsp;
        <input
          class="inputli"
          v-model="paper_params.pyear"
          id="pyear"
          placeholder="pyear" /><br/>
      </div>
    </form>
  </div>
  <br/>
  <div class='blocktitle thick'>查询结果</div>
  <div class='blocktext Plaintext'><ul><PaperItem
    v-for="(paper, index) in papers"
    :pid="paper['pid']"
    :pname="paper['pname']"
    :psource="paper['psource']"
    :pyear="paper['pyear']"
    :ptype="paper['ptype']"
    :level="paper['level']"
    @remove="removePaper(index)" />
  </ul></div>
</template>

<style scoped>
.inputli{width: 30%}
button{width: 6rem;height: 2.5rem;margin:.4rem}
</style>
