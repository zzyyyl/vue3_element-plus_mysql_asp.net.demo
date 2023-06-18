<script setup lang="ts">
import { defineComponent, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './items/NumberInput.vue'
import StringInput from './items/StringInput.vue'
import SelectOptions from './items/SelectOptions.vue'
import DangerButton from './items/DangerButton.vue'

type ElFormCtx = InstanceType<typeof ElForm>

const cnature_options = [
  { value: 1, label: '本科生课程' },
  { value: 2, label: '研究生课程' }
]
const cterm_options = [
  { value: 1, label: '春季学期' },
  { value: 2, label: '夏季学期' },
  { value: 3, label: '秋季学期' }
]
const orderby_options = [
  { value: 'cid', label: '课程编号' },
  { value: 'cname', label: '课程名称' },
  { value: 'cterm', label: '开课学期' },
  { value: 'cnature', label: '课程性质' }
]
const course_form_ref = ref<ElFormCtx>()
</script>

<script lang="ts">
interface iData {
  course_params: {
    cid: string | null
    cname: string | null
    cterm: number | null
    cnature: number | null
    orderby: 'cid' | 'cname' | 'cterm' | 'cnature'
    desc: boolean
    limit: number
  }
}
export default defineComponent({
  data(): iData {
    return {
      course_params: {
        cid: null,
        cname: null,
        cterm: null,
        cnature: null,
        orderby: 'cid',
        desc: false,
        limit: 30
      }
    }
  },
  methods: {
    async submit(course_form_ref: Ref<ElFormCtx>, method: 'get') {
      unref(course_form_ref)
        .validate()
        .then(() => this.$emit(method, this.course_params))
        .catch(() => {})
    }
  }
})
</script>

<template>
  <el-form ref="course_form_ref" :model="course_params" label-width="8rem">
    <el-row justify="space-evenly">
      <el-col :span="22">
        <div>
          <el-form-item
            label="课程编号&nbsp;"
            prop="cid"
            style="width: 98%; height: 2rem"
          >
            <string-input v-model="course_params.cid" id="cid" />
          </el-form-item>
          <el-form-item
            label="课程名称&nbsp;"
            prop="cname"
            style="width: 98%; height: 2rem"
          >
            <string-input v-model="course_params.cname" id="cname" />
          </el-form-item>
          <el-form-item
            label="开课学期&nbsp;"
            prop="cterm"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="course_params.cterm"
              id="cterm"
              :options="cterm_options"
            />
          </el-form-item>
          <el-form-item
            label="课程性质&nbsp;"
            prop="cnature"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="course_params.cnature"
              id="cnature"
              :options="cnature_options"
            />
          </el-form-item>
          <el-form-item
            label="排序方式&nbsp;"
            prop="orderby"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="course_params.orderby"
              id="orderby"
              :options="orderby_options"
            />
          </el-form-item>
          <el-form-item
            label="排序顺序&nbsp;"
            prop="desc"
            style="width: 98%; height: 2rem"
          >
            <el-radio-group v-model="course_params.desc" id="desc">
              <el-radio :label="false">升序</el-radio>
              <el-radio :label="true">降序</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item
            label="显示数量&nbsp;"
            prop="limit"
            style="width: 98%; height: 2rem"
          >
            <number-input v-model="course_params.limit" id="limit" :max="100" />
          </el-form-item>
        </div>
      </el-col>
    </el-row>
    <el-row justify="space-evenly">
      <el-col :span="11">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @click="submit(course_form_ref, 'get')"
          >检 索</el-button
        >
      </el-col>
      <el-col :span="11">
        <danger-button
          label="清 空"
          message="是否清空表单？"
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @commit="course_form_ref.resetFields()"
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
