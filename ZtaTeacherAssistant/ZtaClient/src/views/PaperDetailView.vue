<script setup lang="ts">
import DangerButton from '../components/items/DangerButton.vue'
import NumberInput from '@/components/items/NumberInput.vue'
import SelectOptions from '@/components/items/SelectOptions.vue'

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

interface iAuthor {
  pid: number,
  tid: string,
  ptrank: number | null,
  correspond: boolean | null,
}

interface iPaper {
  pid: number,
  pname: string | null,
  psource: string | null,
  pyear: string | null,
  ptype: number | null,
  level: number | null
}

interface iPaperDetail {
  authors: iAuthor[],
  paper: iPaper
}

export default {
  data() {
    return {
      papers: {} as iPaperDetail,
      origin_papers: {} as iPaperDetail,
      author_editable: false,
      paper_editable: false
    }
  },
  methods: {
    sortAuthor(index: number = -1) {
      if (index >= 0) {
        let ptrank = this.papers.authors[index].ptrank || 0
        let aindex = ptrank > 0 ? ptrank - 1 : 0
        if (this.papers.authors.length > aindex) {
          this.papers.authors[aindex].ptrank = index + 1
        }
      }
      this.papers.authors.sort((a: iAuthor, b: iAuthor) => (a.ptrank ?? 0) - (b.ptrank ?? 0))
      this.papers.authors.forEach((author: iAuthor, index: number) => {
        author.ptrank = index + 1
      })
    },
    onAddAuthor() {
      this.papers.authors.push({
        pid: this.papers.paper.pid,
        tid: '',
        ptrank: this.papers.authors.length + 1,
        correspond: false
      })
    },
    removeAuthor(index: number) {
      this.papers.authors.splice(index, 1)
      this.sortAuthor()
    },
    handleCorrespond(index: number, value: boolean = true) {
      if (value) this.papers.authors.forEach(author => author.correspond = false)
      this.papers.authors[index].correspond = value
    },
    //放弃更改
    onAuthorReset() {
      this.author_editable = false
      this.papers.authors = JSON.parse(JSON.stringify(this.origin_papers.authors))
    },
    //提交更改
    onAuthorSubmit() {
      const pid = this.$route.params.pid
      axios.post(`/api/publish-paper/${pid}`, this.papers.authors)
        .then((res: any) => {
          this.author_editable = false
          ElMessage.success({showClose: true,message: '论文信息更新成功'})
          this.origin_papers.authors = JSON.parse(JSON.stringify(this.papers.authors))
        })
        .catch((err: any) => {
          ElMessage.error({showClose: true,message: `论文信息更新失败，${err.response.data.msg ?? err}`})
        })
    },
    onPaperReset() {
      this.paper_editable = false
      this.papers.paper = JSON.parse(JSON.stringify(this.origin_papers.paper))
    },
    onPaperSubmit() {
      const pid = this.$route.params.pid
      axios.put('/api/paper', {
        pid: pid ?? null,
        pname: this.papers.paper.pname || null,
        psource: this.papers.paper.psource || null,
        pyear: this.papers.paper.pyear || null,
        ptype: this.papers.paper.ptype || null,
        level: this.papers.paper.level || null
      }).then(() => {
        this.paper_editable = false
        ElMessage.success({showClose: true,message: `论文信息更新成功`, duration: 1000})
        let key: keyof iPaper
        for (key in this.papers.paper) {
          let old_val = this.origin_papers.paper[key]
          if (old_val !== null)
            (<typeof old_val>this.papers.paper[key]) ??= old_val
        }
        this.origin_papers.paper = JSON.parse(JSON.stringify(this.papers.paper))
      }).catch((err: any) => {
        ElMessage.error({showClose: true,message: `论文信息更新失败，${err.response.data.msg ?? err}`})
      })
    }
  },
  mounted() {
    const pid = this.$route.params.pid
    if (pid) {
      axios.get(`/api/publish-paper/${pid}`)
        .then((res: any) => {
          this.papers = res.data
          this.papers.authors.sort((a: iAuthor, b: iAuthor) => (a.ptrank ?? 0) - (b.ptrank ?? 0))
          this.papers.authors.forEach((author: iAuthor, index: number) => {
            author.ptrank = index + 1
            author.correspond = author.correspond ?? false
          })
          this.origin_papers = JSON.parse(JSON.stringify(this.papers))
        })
        .catch((err: any) => {
          ElMessage.error({showClose: true,message: `论文检索失败，${err.response.data.msg ?? err}`})
        })
    }
  }
}

</script>

<template>
  <div class='blocktitle thick'>论文详情</div>
  <div class='blocktext Plaintext'>
    <el-table :data="[papers.paper]">
      <el-table-column fixed prop="pid" label="编号" width="60"></el-table-column>
      <el-table-column prop="pname" label="名称" width="auto">
        <template #default="scope">
          <el-input v-if="paper_editable" v-model="papers.paper.pname"></el-input>
          <span v-else>{{ scope.row?.pname }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="psource" label="来源" width="100">
        <template #default="scope">
          <el-input v-if="paper_editable" v-model="papers.paper.psource"></el-input>
          <span v-else>{{ scope.row?.psource }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="pyear" label="年份" width="150">
        <template #default="scope">
          <el-date-picker v-if="paper_editable" type="date" value-format="YYYY-MM-DD" v-model="papers.paper.pyear" style="width: auto;" />
          <span v-else>{{ scope.row?.pyear }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="ptype" label="类型" width="150">
        <template #default="scope">
          <select-options
            v-if="paper_editable"
            v-model="papers.paper.ptype"
            :options="ptype_options" />
          <span v-else>{{ ptype_options.find(type => type.value === scope.row?.ptype)?.label }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="level" label="级别" width="150">
        <template #default="scope">
          <select-options
            v-if="paper_editable"
            v-model="papers.paper.level"
            :options="level_options" />
          <span v-else>{{ level_options.find(level => level.value === scope.row?.level)?.label }}</span>
        </template>
      </el-table-column>
    </el-table>
    <el-row justify="space-between">
      <el-col v-if="!paper_editable">
        <el-button
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @click="paper_editable=true">启用编辑</el-button>
      </el-col>
      <el-col :span="11" v-if="paper_editable">
        <danger-button
          label="放弃更改"
          message="是否放弃更改？"
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @commit="onPaperReset" />
      </el-col>
      <el-col :span="11" v-if="paper_editable">
        <danger-button
          label="确认更改"
          message="是否确认更改？"
          type="primary"
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @commit="onPaperSubmit" />
      </el-col>
    </el-row>

    <br />
    <el-table :data="papers.authors" max-height="500">
      <el-table-column label="作者排名">
        <template #default="scope">
          <number-input v-if="author_editable" class="inputli" v-model="scope.row.ptrank" placeholder="请输入作者排名" @blur="sortAuthor(scope.$index)" />
          <el-tag v-else>第 {{ scope.row.ptrank }} 作者</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="tid" label="作者编号">
        <template #default="{row}">
          <el-input v-if="author_editable" class="inputli" v-model="row.tid" placeholder="请输入作者编号"></el-input>
          <label v-else> {{ row.tid }} </label>
        </template>
      </el-table-column>
      <el-table-column label="是否通讯作者">
        <template #default="scope">
          <el-tag v-if="scope.row.correspond" @click="author_editable && handleCorrespond(scope.$index, false)">通讯作者</el-tag>
          <el-tag v-else type="info" @click="author_editable && handleCorrespond(scope.$index)">非通讯作者</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" fixed="right" width="100">
        <template #default="scope">
          <danger-button v-if="author_editable" label="删 除" message="是否删除该作者？" @commit="removeAuthor(scope.$index)"/>
          <el-button v-else plain type="primary" @click="$router.push(`/teacher/${scope.row.tid}`)">详 情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-row justify="space-between">
      <el-col v-if="!author_editable">
        <el-button
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @click="author_editable=true">启用编辑</el-button>
      </el-col>
      <el-col :span="7" v-if="author_editable">
        <el-button style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0" @click="onAddAuthor">增加作者</el-button>
      </el-col>
      <el-col :span="7" v-if="author_editable">
        <danger-button
          label="放弃更改"
          message="是否放弃更改？"
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @commit="onAuthorReset" />
      </el-col>
      <el-col :span="7" v-if="author_editable">
        <danger-button
          label="确认更改"
          message="是否确认更改？"
          type="primary"
          style="width: 100%;height: 2.5rem;margin: 1.5rem 0 .5rem 0"
          @commit="onAuthorSubmit" />
      </el-col>
    </el-row>
    <!-- {{ papers }} -->
  </div>
</template>

<style scoped>
.inputli{width: 99%;border: 0;}
</style>
