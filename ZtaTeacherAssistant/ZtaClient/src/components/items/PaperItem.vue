<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  props: ['pid', 'pname', 'psource', 'pyear', 'ptype', 'level'],
  emits: ['delete'],
  data() {
    return {
      popoverVisible: false
    }
  },
  methods: {
    handleClick(sure: boolean) {
      sure && this.$emit('delete')
      this.popoverVisible = false
    }
  }
})
</script>

<template>
  <li style="margin: 1rem 0;">
    <el-row justify="space-evenly" align="middle">
      <el-col :span="18">
        <li>论文编号： {{ pid }}</li>
        <li>论文名称： {{ pname }}</li>
        <li>论文来源： {{ psource }}</li>
        <li>论文年份： {{ pyear }}</li>
        <li>论文类别： {{ ptype }}</li>
        <li>论文级别： {{ level }}</li>
      </el-col>
      <el-col :span="4">
        <div style="text-align: center;height: 100%;align-items: center;">
          <el-popover :visible="popoverVisible" placement="top" :width="180">
            <p>是否确认删除该论文？</p>
            <div style="text-align: right; margin: 0">
              <el-button size="small" text @click="handleClick(false)">取 消</el-button>
              <el-button size="small" type="primary" @click="handleClick(true)">确 定</el-button>
            </div>
            <template #reference>
              <el-button plain class="myplain" type="danger" @click="popoverVisible = true">删 除</el-button>
            </template>
          </el-popover>
        </div>
      </el-col>
    </el-row>
  </li>
</template>

<style scoped>
li {
  list-style: none;
  font-family: 'Consolas', 'Code Normal Font', 微软雅黑, monospace;
  font-size: 16px;
}
</style>
