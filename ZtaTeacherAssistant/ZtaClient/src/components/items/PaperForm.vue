<script setup lang="ts">
import { defineComponent, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './NumberInput.vue'
import StringInput from './StringInput.vue'
import SelectOptions from './SelectOptions.vue'

type ElFormCtx = InstanceType<typeof ElForm>

// const emits = defineEmits(['get', 'post', 'put'])
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
const paper_params_rules = {
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
const paper_form_ref = ref<ElFormCtx>()
</script>

<script lang="ts">
interface iData {
  paper_params: {
    pid: number | null,
    pname: string | null,
    psource: string | null,
    pyear: string | null,
    ptype: number | null,
    level: number | null
  }
}
export default defineComponent({
  data() : iData {
    return {
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
        .catch(() => {})
    }
  }
})
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
            <string-input v-model="paper_params.psource" id="psource" />
          </el-form-item>
          <el-form-item label="论文年份&nbsp;" prop="pyear">
            <el-date-picker type="date" value-format="YYYY-MM-DD" v-model="paper_params.pyear" id="pyear" style="width: 98%;" />
          </el-form-item>
        </div>
      </el-col>
      <el-col :span="4">
        <div style="text-align: center;"><br/><br/>
          <el-button @click="submit(paper_form_ref, 'get')">检 索</el-button><br/><br/>
          <el-button @click="submit(paper_form_ref, 'post')">登 记</el-button><br/><br/>
          <!-- <el-button @click="submit(paper_form_ref, 'put')">修 改</el-button><br/><br/> -->
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
