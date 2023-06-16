<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  emits: ['commit'],
  data() {
    return {
      popoverVisible: false
    }
  },
  methods: {
    handleClick(sure: boolean) {
      sure && this.$emit('commit')
      this.popoverVisible = false
    }
  }
})
</script>

<script setup lang="ts">
import { defineProps, type StyleValue } from 'vue'

const props = withDefaults(defineProps<{
  message: string,
  label: string,
  style?: StyleValue,
  type?: string,
}>(), {
  style: 'margin: .4rem 0',
  type: 'danger'
})

</script>

<template>
  <el-popover :visible="popoverVisible" placement="top" :width="180">
    <p>{{ message }}</p>
    <div style="text-align: right; margin: 0">
      <el-button size="small" text @click="handleClick(false)">取 消</el-button>
      <el-button size="small" type="primary" @click="handleClick(true)">确 定</el-button>
    </div>
    <template #reference>
      <el-button plain :type="type" @click="popoverVisible = true" :style="style">{{ label }}</el-button>
    </template>
  </el-popover>
</template>