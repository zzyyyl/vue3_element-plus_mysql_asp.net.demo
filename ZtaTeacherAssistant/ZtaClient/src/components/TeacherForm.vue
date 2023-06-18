<script setup lang="ts">
import { defineComponent, ref, unref } from 'vue'
import type { Ref } from 'vue'
import { ElForm } from 'element-plus'
import NumberInput from './items/NumberInput.vue'
import StringInput from './items/StringInput.vue'
import SelectOptions from './items/SelectOptions.vue'
import DangerButton from './items/DangerButton.vue'

type ElFormCtx = InstanceType<typeof ElForm>

const gender_options = [
  { value: 1, label: '男' },
  { value: 2, label: '女' }
]

const title_options = [
  { value: 1, label: '博士后' },
  { value: 2, label: '助教' },
  { value: 3, label: '讲师' },
  { value: 4, label: '副教授' },
  { value: 5, label: '特任教授' },
  { value: 6, label: '教授' },
  { value: 7, label: '助理研究员' },
  { value: 8, label: '特任副研究员' },
  { value: 9, label: '副研究员' },
  { value: 10, label: '特任研究员' },
  { value: 11, label: '研究员' }
]

const orderby_options = [
  { value: 'tid', label: '教师编号' },
  { value: 'tname', label: '教师姓名' },
  { value: 'gender', label: '教师性别' },
  { value: 'title', label: '教师职称' },
]

const teacher_form_ref = ref<ElFormCtx>()
</script>

<script lang="ts">
interface iData {
  teacher_params: {
    tid: string | null
    tname: string | null
    gender: number | null
    title: number | null
    orderby: 'tid' | 'tname' | 'gender' | 'title'
    desc: boolean
    limit: number
  }
}
export default defineComponent({
  data(): iData {
    return {
      teacher_params: {
        tid: null,
        tname: null,
        gender: null,
        title: null,
        orderby: 'tid',
        desc: false,
        limit: 30
      }
    }
  },
  methods: {
    async submit(teacher_form_ref: Ref<ElFormCtx>, method: 'get') {
      unref(teacher_form_ref)
        .validate()
        .then(() => this.$emit(method, this.teacher_params))
        .catch(() => {})
    }
  }
})
</script>

<template>
  <el-form ref="teacher_form_ref" :model="teacher_params" label-width="8rem">
    <el-row justify="space-evenly">
      <el-col :span="22">
        <div>
          <el-form-item
            label="教师编号&nbsp;"
            prop="tid"
            style="width: 98%; height: 2rem"
          >
            <string-input v-model="teacher_params.tid" id="tid" />
          </el-form-item>
          <el-form-item
            label="教师姓名&nbsp;"
            prop="tname"
            style="width: 98%; height: 2rem"
          >
            <string-input v-model="teacher_params.tname" id="tname" />
          </el-form-item>

          <el-form-item
            label="教师性别&nbsp;"
            prop="gender"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="teacher_params.gender"
              :options="gender_options"
            />
          </el-form-item>
          <el-form-item
            label="教师职称&nbsp;"
            prop="title"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="teacher_params.title"
              :options="title_options"
            />
          </el-form-item>
          <el-form-item
            label="排序方式&nbsp;"
            prop="orderby"
            style="width: 98%; height: 2rem"
          >
            <select-options
              v-model="teacher_params.orderby"
              id="orderby"
              :options="orderby_options"
            />
          </el-form-item>
          <el-form-item
            label="排序顺序&nbsp;"
            prop="desc"
            style="width: 98%; height: 2rem"
          >
            <el-radio-group v-model="teacher_params.desc" id="desc">
              <el-radio :label="false">升序</el-radio>
              <el-radio :label="true">降序</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item
            label="显示数量&nbsp;"
            prop="limit"
            style="width: 98%; height: 2rem"
          >
            <number-input v-model="teacher_params.limit" id="limit" :max="100" />
          </el-form-item>
        </div>
      </el-col>
    </el-row>
    <el-row justify="space-evenly">
      <el-col :span="11">
        <el-button
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @click="submit(teacher_form_ref, 'get')"
          >检 索</el-button
        >
      </el-col>
      <el-col :span="11">
        <danger-button
          label="清 空"
          message="是否清空表单？"
          style="width: 100%; height: 2.5rem; margin: 0 0 0.5rem 0"
          @commit="teacher_form_ref.resetFields()"
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
