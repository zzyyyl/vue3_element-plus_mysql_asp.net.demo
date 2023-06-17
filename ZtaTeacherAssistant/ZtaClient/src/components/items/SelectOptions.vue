<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  emits: ['update:modelValue'],
  props: {
    modelValue: {},
    options: {
      type: Array<{
        value: any
        label: string
      }>,
      default: []
    }
  },
  data(): { choice: any } {
    return {
      choice: null
    }
  },
  watch: {
    modelValue(val: any) {
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
  },
  mounted() {
    this.choice = this.modelValue
  }
})
</script>

<template>
  <el-select style="width: 100%" v-model="choice" clearable @clear="temp">
    <el-option
      class="Code"
      v-for="item in options"
      :key="item.value"
      :label="item.label"
      :value="item.value"
    />
  </el-select>
</template>
