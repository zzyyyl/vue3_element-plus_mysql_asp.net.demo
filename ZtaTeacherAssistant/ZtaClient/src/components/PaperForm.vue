<script lang="ts">
import axios from 'axios'
import PaperItem from './PaperItem.vue'

export default {
  name: 'App',
  components: {
    PaperItem
  },
  props: {},
  data() {
    return {
      models: {
        paper: '/paper',
        teacher: '/teacher'
      },
      paper_params: {
        pid: null,
        pname: null,
        psource: null,
        pyear: null,
        ptype: null,
        level: null
      },
    }
  },
  emits: [ 'data' ],
  methods: {
    getPaper() {
      // get /api/paper
      ;
      axios.get('/api/paper', {
        params: {
          pid: this.paper_params.pid ? this.paper_params.pid : null,
          pname: this.paper_params.pname ? this.paper_params.pname : null,
          psource: this.paper_params.psource ? this.paper_params.psource : null,
          pyear: this.paper_params.pyear ? this.paper_params.pyear : null,
          ptype: this.paper_params.ptype ? this.paper_params.ptype : null,
          level: this.paper_params.level ? this.paper_params.level : null
        },
      }).then((res: any) => {
        console.log(res.data)
        this.$emit(res.data)
      }).catch((err: any) => {
        console.log(err)
      })
    }
  }
}

</script>

<template><form v-on:submit.prevent="getPaper">
    <div style="float: right; width: 45%">
        <label for="pid">论文编号</label><br/>
        <input
        v-model="paper_params.pid"
        id="pid"
        placeholder="pid" /><br/>
        <label for="pid">论文名</label><br/>
        <input
        v-model="paper_params.pname"
        id="get-paper"
        placeholder="pname" /><br/>
        <label for="pid">论文来源</label><br/>
        <input
        v-model="paper_params.psource"
        id="get-paper"
        placeholder="psource" /><br/>
    </div>
    <div style="width: 45%">
        <label for="pid">论文年份</label><br/>
        <input
        v-model="paper_params.pyear"
        id="get-paper"
        placeholder="pyear" /><br/>
        <label for="pid">论文类别</label><br/>
        <input
        v-model="paper_params.ptype"
        id="get-paper"
        placeholder="ptype" /><br/>
        <label for="pid">论文级别</label><br/>
        <input
        v-model="paper_params.level"
        id="get-paper"
        placeholder="level" /><br/>
        </div>
        <br/>
    <button>查询论文</button>
</form></template>