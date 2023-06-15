<script lang="ts">
import { defineComponent } from 'vue'
export default defineComponent({
  emits: ['update:modelValue'],
  data() {
    return {
      num: null
    }
  },
  watch: {
    num(val: string) {
      if (!val) {
        this.$emit('update:modelValue', null)
      } else {
        this.$emit(
          'update:modelValue', 
          Math.min(2147483647,
                   parseInt(val.replace(/[^\d]/g, '')))
        )
      }
    }
  }
})
</script>

<template>
  <el-input
    style="width: 98%;border: 0"
    type="number"
    min=0 max=2147483647
    v-model="num" />
</template>
