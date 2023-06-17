<script setup lang="ts">
import { defineComponent, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './items/NumberInput.vue'
import StringInput from './items/StringInput.vue'
import SelectOptions from './items/SelectOptions.vue'
import DangerButton from './items/DangerButton.vue'

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
const orderby_options = [
  { value: 'pid', label: '论文编号' },
  { value: 'pname', label: '论文名称' },
  { value: 'pyear', label: '发表年份' },
  { value: 'ptype', label: '论文类型' },
  { value: 'level', label: '论文级别' },
  { value: 'psource', label: '论文来源' }
]
const paper_form_ref = ref<ElFormCtx>()
</script>

<script lang="ts">
interface iData {
  method: 'get' | 'post'
  paper_params: {
    pid: number | null
    pname: string | null
    psource: string | null
    pyear: string | string[] | null
    ptype: number | null
    level: number | null
    orderby: 'pid' | 'pname' | 'pyear' | 'ptype' | 'level' | 'psource'
    desc: boolean
    limit: number
  }
}
export default defineComponent({
  data(): iData {
    return {
      method: 'get',
      paper_params: {
        pid: null,
        pname: null,
        psource: null,
        pyear: null,
        ptype: null,
        level: null,
        orderby: 'pid',
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
        if (this.paper_params.pyear && Array.isArray(this.paper_params.pyear)) {
          this.paper_params.pyear = this.paper_params.pyear[0]
        }
      } else {
        this.method = 'get'
        if (this.paper_params.pyear && typeof this.paper_params.pyear === 'string') {
          this.paper_params.pyear = [this.paper_params.pyear, this.paper_params.pyear]
        }
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
          <el-form-item label="论文编号&nbsp;" prop="pid" style="width: 98%; height: 2rem">
            <number-input v-model="paper_params.pid" id="pid" />
          </el-form-item>
          <el-form-item label="论文名称&nbsp;" prop="pname" style="width: 98%; height: 2rem">
            <string-input v-model="paper_params.pname" id="pname" />
          </el-form-item>
          <el-form-item label="论文类型&nbsp;" prop="ptype" style="width: 98%; height: 2rem">
            <select-options v-model="paper_params.ptype" id="ptype" :options="ptype_options" />
          </el-form-item>
          <el-form-item label="论文级别&nbsp;" prop="level" style="width: 98%; height: 2rem">
            <select-options v-model="paper_params.level" id="level" :options="level_options" />
          </el-form-item>
          <el-form-item label="论文来源&nbsp;" prop="psource" style="width: 98%; height: 2rem">
            <string-input v-model="paper_params.psource" id="psource" />
          </el-form-item>
          <el-form-item label="论文年份&nbsp;" prop="pyear" style="width: 98%; height: 2rem">
            <el-date-picker
              v-if="method === 'get'"
              type="daterange"
              value-format="YYYY-MM-DD"
              v-model="paper_params.pyear"
              range-separator="To"
              unlink-panels
              start-placeholder="Start date"
              end-placeholder="End date"
              style="width: 100%"
            />
            <el-date-picker
              v-else
              type="date"
              value-format="YYYY-MM-DD"
              v-model="paper_params.pyear"
              placeholder="Select date"
              style="width: 100%"
            />
          </el-form-item>
          <el-form-item
            v-if="method === 'get'"
            label="排序方式&nbsp;"
            prop="orderby"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="paper_params.orderby"
              id="orderby"
              :options="orderby_options"
            />
          </el-form-item>
          <el-form-item
            v-if="method === 'get'"
            label="排序顺序&nbsp;"
            prop="desc"
            style="width: 98%; height: 2rem"
          >
            <el-radio-group v-model="paper_params.desc" id="desc">
              <el-radio :label="false">升序</el-radio>
              <el-radio :label="true">降序</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item
            v-if="method === 'get'"
            label="显示数量&nbsp;"
            prop="limit"
            style="width: 98%; height: 2rem"
          >
            <number-input v-model="paper_params.limit" id="limit" :max="100" />
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
