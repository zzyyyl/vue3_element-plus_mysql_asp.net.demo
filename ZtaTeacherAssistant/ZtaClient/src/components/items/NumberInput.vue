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
  methods: {
    updateNum() {
      console.log('update', this.num)
      if (!this.num && this.num !== 0) {
        this.num = null
      } else {
        this.num = Math.max(this.min, Math.min(this.max, this.num))
      }
      this.$emit('update:modelValue', this.num)
    }
  }
})
</script>

<template>
  <el-input
    style="width: 100%"
    type="number"
    :min="min" :max="max"
    v-model="num" @input="updateNum" />
</template>
