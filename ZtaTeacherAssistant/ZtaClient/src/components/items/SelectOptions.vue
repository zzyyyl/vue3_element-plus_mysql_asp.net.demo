<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  emits: ['update:modelValue'],
  props: {
    modelValue: {
      type: Number,
      default: null
    },
    options: {
      type: Array<{
        value: number,
        label: string
      }>,
      default: []
    }
  },
  data() : { choice: number | null } {
    return {
      choice: null
    }
  },
  watch: {
    modelValue(val: number | null) {
      this.choice = val
    },
    choice(val: number) {
      this.$emit('update:modelValue', val || null)
    }
  },
  methods: {
    temp() {
      console.log('clear')
      this.choice = null
    }
  }
})
</script>

<template>
  <el-select style="width: 98%;border: 0" v-model="choice" clearable @clear="temp">
    <el-option
      class="Code"
      v-for="item in options"
      :key="item.value"
      :label="item.label"
      :value="item.value" />
  </el-select>
</template>
