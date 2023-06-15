<script lang="ts">
import axios from 'axios'
import PaperItem from '../items/PaperItem.vue'
import { ref } from 'vue'
import { ElForm } from 'element-plus'

type ElFormCtx = InstanceType<typeof ElForm>

interface iPaperParams {
  pid: number | string | null,
  pname: string | null,
  psource: string | null,
  pyear: string | null,
  ptype: string | null,
  level: string | null
}
interface iData {
  remove_dialog_visiable: boolean,
  remove_paper_index: number | null,
  level_options: { value: number | null, label: string }[],
  ptype_options: { value: number | null, label: string }[],
  paper_params_rules: any,
  submit: CallableFunction | null,
  paper_params: iPaperParams,
  papers: iPaperParams[]
}

export default {
  components: {
    PaperItem
  },
  props: {},
  data() : iData {
    return {
      remove_dialog_visiable: false,
      remove_paper_index: null,
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
      paper_params_rules: {},
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
      this.remove_paper_index = index
      this.remove_dialog_visiable = true
    },
    confirmRemovePaper() {
      let index = this.remove_paper_index
      this.remove_paper_index = null
      if (index === null) {
        alert('请选择要删除的论文')
        return
      }
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
    async beforeSubmitForm(paper_form_ref: any, callback: CallableFunction) {
      paper_form_ref.validate()
      .then(callback)
      .catch((err: any) => {})
    },
    close_remove_dialog() {
      this.remove_dialog_visiable = false
    }
  },
  setup() {
    const paper_form_ref = ref<ElFormCtx>()
    return {
      paper_form_ref
    }
  },
  mounted() {
    this.paper_params_rules = {
      pid: [
        { validator: (rule: any, value: any, callback: CallableFunction) => {
          if (value === null || value === undefined || value === '') {
            callback()
          } else if (value >= 0 && value < 2147483647) {
            callback()
          } else {
            callback(new Error('请输入正确的论文编号'))
          }
        }, trigger: 'blur' }
      ],
      pyear: [
        { validator: (rule: any, value: any, callback: CallableFunction) => {
          if (value === null || value === undefined || value === '') {
            callback()
          } else if (Date.parse(value) < Date.now()) {
            callback()
          } else {
            callback(new Error('日期须在今天以前'))
          }
        }, trigger: 'blur' }
      ],
    }
  }
}

</script>

<template>
  <!--确认对话框-->
  <el-dialog
    title="确认删除"
    v-model="remove_dialog_visiable"
    width="30rem"
    :show-close="false">
    <div style="display: block; text-align: center;">
    <span>是否确认删除该论文？</span><br /><br />
    <span class="dialog-footer" style="width: 30%;">
      <el-button type="info" style="margin:.4rem 2rem" @click="close_remove_dialog">取 消</el-button>
      <el-button type="danger" style="margin:.4rem 2rem" @click="{confirmRemovePaper(); close_remove_dialog()}">确 定</el-button>
    </span>
    </div>
  </el-dialog>
  <div class='blocktitle thick'>论文查询</div>
  <div class='blocktext Plaintext'>
    <el-form
      ref="paper_form_ref"
      :model="paper_params"
      :rules="paper_params_rules"
      label-width="8rem">
      <div style="float: right; width: 20%; text-align: center;">
        <el-button @click="beforeSubmitForm(paper_form_ref, getPaper)">检 索</el-button>
        <br/>
        <br/>
        <el-button @click="beforeSubmitForm(paper_form_ref, postPaper)">登 记</el-button>
        <br/>
        <br/>
        <el-button @click="beforeSubmitForm(paper_form_ref, putPaper)">修 改</el-button>
        <br/>
        <br/>
        <el-button type="danger" plain @click="paper_form_ref.resetFields()">清 空</el-button>
      </div>
      <div style="width: 80%; text-align: center;">
        <el-form-item label="论文编号&nbsp;" prop="pid">
          <el-input
            class="inputli"
            @blur="() => { if (paper_params.pid === '') paper_params.pid = null }"
            type="number"
            min=0
            max=2147483647
            v-model.number="paper_params.pid"
            id="pid" />
        </el-form-item>
        <el-form-item label="论文名称&nbsp;" prop="pname">
          <el-input
            class="inputli"
            v-model="paper_params.pname"
            id="pname" />
        </el-form-item>
        <el-form-item label="论文类型&nbsp;" prop="ptype">
          <el-select
            class="inputli"
            v-model="paper_params.ptype"
            id="ptype" >
            <el-option
              v-for="item in ptype_options"
              :key="item.value"
              :label="item.label"
              :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="论文级别&nbsp;" prop="level">
          <el-select
            class="inputli"
            v-model="paper_params.level"
            id="level" >
            <el-option
              v-for="item in level_options"
              :key="item.value"
              :label="item.label"
              :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="论文来源&nbsp;" prop="psource">
          <el-input
            class="inputli"
            v-model="paper_params.psource"
            id="psource" />
        </el-form-item>
        <el-form-item label="论文年份&nbsp;" prop="pyear">
          <el-input
            class="inputli"
            v-model="paper_params.pyear"
            id="pyear" />
        </el-form-item>
      </div>
    </el-form>
  </div>
  <br/>
  <div class='blocktitle thick'>查询结果</div>
  <div class='blocktext Plaintext'><ul><paper-item
    v-for="(paper, index) in papers"
    :key="index"
    :pid="paper.pid"
    :pname="paper.pname"
    :psource="paper.psource"
    :pyear="paper.pyear"
    :ptype="ptype_options.find(ptype => ptype.value === paper.ptype)?.label"
    :level="level_options.find(level => level.value === paper.level)?.label"
    @remove="removePaper(index)" />
  </ul></div>
</template>

<style scoped>
.inputli{width: 99%;border: 0;}
button{width: 5rem;height: 2.5rem;margin:.6rem 0}
</style>
