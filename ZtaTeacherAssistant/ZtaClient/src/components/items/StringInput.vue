<script lang="ts">
import { defineComponent } from 'vue'
export default defineComponent({
  emits: ['update:modelValue'],
  data() {
    return {
      str: null
    }
  },
  watch: {
    str(val: string) {
      this.$emit('update:modelValue', val || null)
    }
  }
})
</script>

<template>
  <el-input style="width: 100%" v-model="str" />
</template>
