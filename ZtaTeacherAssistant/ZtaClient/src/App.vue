<script lang="ts">
import axios from 'axios'
import PaperItem from './components/PaperItem.vue'
import Hitokoto from './components/Hitokoto.vue'

export default {
  name: 'App',
  components: {
    PaperItem,
    Hitokoto
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
      papers: [],
    }
  },
  created() {
    const picTotal = 13;
    let randBgIndex = Math.floor(Math.random() * picTotal) % picTotal;
    document.body.style.backgroundImage = "url(backgroundpics/" + randBgIndex + ".jpg)";
    document.body.style.backgroundPosition = "center center";
    document.body.style.backgroundRepeat = "no-repeat";
  },
  methods: {
    getPaper() {
      // get /api/paper
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
        this.papers = res.data
      }).catch((err: any) => {
        console.log(err)
      })
    }
  }
}

</script>

<template>
  <nav class='maintitle'><ul>
    <li class='EnglishP thick maintitletext'><a href='/'>ZTA</a></li>
    <li v-for="(value, key) in models" class="Code">
      <a :href="value">{{ key }}</a>
    </li>
	</ul></nav>
    <div class="transbox">
      <div class="sidebar"><Hitokoto /></div>
      <div class="mainbar">
      <div class='blocktitle thick'>论文查询</div>
      <div class='blocktext Plaintext'>
      <form v-on:submit.prevent="getPaper">
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
        @remove="papers.splice(index, 1)" />
      </ul></div>
  </div>
</div>
</template>

<style scoped>
</style>
