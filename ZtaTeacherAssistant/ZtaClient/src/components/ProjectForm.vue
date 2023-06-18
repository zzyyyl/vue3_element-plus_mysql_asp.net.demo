<script setup lang="ts">
import { defineComponent, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './items/NumberInput.vue'
import StringInput from './items/StringInput.vue'
import SelectOptions from './items/SelectOptions.vue'
import DangerButton from './items/DangerButton.vue'

type ElFormCtx = InstanceType<typeof ElForm>

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
const paper_form_ref = ref<ElFormCtx>()
</script>

<script lang="ts">
interface iData {
  method: 'get' | 'post'
  paper_params: {
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
}
export default defineComponent({
  data(): iData {
    return {
      method: 'get',
      paper_params: {
        jid: null,
        jname: null,
        jsource: null,
        jtype: null,
        jbudgets: null,
        styear: null,
        edyear: null,
        orderby: 'jid',
        desc: false,
        limit: 30
      }
    }
  },
  methods: {
    async submit(paper_form_ref: Ref<ElFormCtx>, method: 'get' | 'post') {
      unref(paper_form_ref)
        .validate()
        .then(() => this.$emit(method, this.paper_params))
        .catch(() => {})
    },
    handleMethodChange() {
      if (this.method === 'get') {
        this.method = 'post'
      } else {
        this.method = 'get'
      }
      this.$emit('method-change', this.method)
    }
  }
})
</script>

<template>
  <el-form ref="paper_form_ref" :model="paper_params" label-width="8rem">
    <el-row justify="space-evenly">
      <el-col :span="22">
        <div>
          <el-form-item label="项目编号&nbsp;" prop="jid" style="width: 98%; height: 2rem">
            <string-input v-model="paper_params.jid" id="jid" />
          </el-form-item>
          <el-form-item label="项目名称&nbsp;" prop="jname" style="width: 98%; height: 2rem">
            <string-input v-model="paper_params.jname" id="jname" />
          </el-form-item>
          <el-form-item label="项目来源&nbsp;" prop="jsource" style="width: 98%; height: 2rem">
            <string-input v-model="paper_params.jsource" id="jsource" />
          </el-form-item>
          <el-form-item label="项目类型&nbsp;" prop="jtype" style="width: 98%; height: 2rem">
            <select-options v-model="paper_params.jtype" id="jtype" :options="jtype_options" />
          </el-form-item>
          <el-form-item label="项目经费&nbsp;" prop="jbudgets" style="width: 98%; height: 2rem">
            <number-input v-model="paper_params.jbudgets" id="jbudgets" />
          </el-form-item>
          <el-form-item label="起始年份&nbsp;" prop="styear" style="width: 98%; height: 2rem">
            <number-input v-model="paper_params.styear" id="styear" />
          </el-form-item>
          <el-form-item label="结束年份&nbsp;" prop="edyear" style="width: 98%; height: 2rem">
            <number-input v-model="paper_params.edyear" id="edyear" />
          </el-form-item>
          <el-form-item v-if="method === 'get'" label="排序方式&nbsp;" prop="orderby" style="width: 98%; height: 2rem">
            <select-options v-model="paper_params.orderby" id="orderby" :options="orderby_options" />
          </el-form-item>
          <el-form-item v-if="method === 'get'" label="排序顺序&nbsp;" prop="desc" style="width: 98%; height: 2rem">
            <el-radio-group v-model="paper_params.desc" id="desc">
              <el-radio :label="false">升序</el-radio>
              <el-radio :label="true">降序</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item v-if="method === 'get'" label="显示数量&nbsp;" prop="limit" style="width: 98%; height: 2rem">
            <number-input v-model="paper_params.limit" id="limit" />
          </el-form-item>
        </div>
      </el-col>
    </el-row>
    <el-row justify="space-evenly">
      <el-col :span="7">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @click="handleMethodChange()"
        >
          {{ method === 'get' ? '切换为登记' : '切换为检索' }}
        </el-button>
      </el-col>
      <el-col :span="7">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @click="submit(paper_form_ref, method)"
        >
          {{ method === 'get' ? '检 索' : '登 记' }}
        </el-button>
      </el-col>
      <el-col :span="7">
        <danger-button
          label="清 空"
          message="是否清空表单？"
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @commit="paper_form_ref.resetFields()"
        />
      </el-col>
    </el-row>
  </el-form>
</template>

<style scoped>
button {
  width: 5rem;
  height: 2.5rem;
  margin: 0.6rem 0;
}
</style>
