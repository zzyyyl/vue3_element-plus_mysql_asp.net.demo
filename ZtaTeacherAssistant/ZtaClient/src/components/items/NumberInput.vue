<script lang="ts">
import { defineComponent, type PropType } from 'vue'
export default defineComponent({
  props: {
    modelValue: {
      type: Number as PropType<number | null>,
      default: null
    },
    max: {
      type: Number,
      default: 2147483647
    },
    min: {
      type: Number,
      default: 0
    }
  },
  emits: ['update:modelValue'],
  data() {
    return {
      num: null as number | null
    }
  },
  watch: {
    num(val: number | null) {
      if (!val && val !== 0) {
        this.num = null
      } else {
        this.num = Math.max(this.min, Math.min(this.max, val))
      }
      this.$emit('update:modelValue', this.num)
    },
    modelValue(val: number | null) {
      this.num = val
    }
  },
  mounted() {
    this.num = this.modelValue
  }
})
</script>

<template>
  <el-input
    style="width: 100%"
    :min="min" :max="max"
    v-model="num" />
</template>
