<script lang="ts">
import { ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './NumberInput.vue'
import StringInput from './StringInput.vue'
import SelectOptions from './SelectOptions.vue'

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
  level_options: { value: number | null, label: string }[],
  ptype_options: { value: number | null, label: string }[],
  paper_params_rules: any,
  paper_params: iPaperParams,
}

export default {
  components: { NumberInput, StringInput, SelectOptions },
  props: {},
  emits: [
    'get',
    'post',
    'put',
  ],
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
      paper_params_rules: {},
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
  methods: {
    async submit(paper_form_ref: Ref<ElFormCtx>, method: 'get' | 'post' | 'put') {
      unref(paper_form_ref)
        .validate()
        .then(() => this.$emit(method, this.paper_params))
        .catch((err: any) => {})
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
          } else if (value >= 0 && value <= 2147483647) {
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
  <el-form
    ref="paper_form_ref"
    :model="paper_params"
    :rules="paper_params_rules"
    label-width="8rem">
    <el-row justify="space-evenly">
      <el-col :span="18">
        <div>
          <el-form-item label="论文编号&nbsp;" prop="pid">
            <number-input v-model="paper_params.pid" id="pid" />
          </el-form-item>
          <el-form-item label="论文名称&nbsp;" prop="pname">
            <string-input v-model="paper_params.pname" id="pname" />
          </el-form-item>
          <el-form-item label="论文类型&nbsp;" prop="ptype">
            <select-options v-model="paper_params.ptype" id="ptype" :options="ptype_options" />
          </el-form-item>
          <el-form-item label="论文级别&nbsp;" prop="level">
            <select-options v-model="paper_params.level" id="level" :options="level_options" />
          </el-form-item>
          <el-form-item label="论文来源&nbsp;" prop="psource">
            <number-input v-model="paper_params.psource" id="psource" />
          </el-form-item>
          <el-form-item label="论文年份&nbsp;" prop="pyear">
            <string-input v-model="paper_params.pyear" id="pyear" />
          </el-form-item>
        </div>
      </el-col>
      <el-col :span="4">
        <div style="text-align: center;">
          <el-button @click="submit(paper_form_ref, 'get')">检 索</el-button><br/><br/>
          <el-button @click="submit(paper_form_ref, 'post')">登 记</el-button><br/><br/>
          <el-button @click="submit(paper_form_ref, 'put')">修 改</el-button><br/><br/>
          <el-button type="danger" plain @click="paper_form_ref.resetFields()">清 空</el-button>
        </div>
      </el-col>
    </el-row>
  </el-form>
</template>

<style scoped>
.inputli{width: 99%;border: 0;}
button{width: 5rem;height: 2.5rem;margin:.6rem 0}
</style>
