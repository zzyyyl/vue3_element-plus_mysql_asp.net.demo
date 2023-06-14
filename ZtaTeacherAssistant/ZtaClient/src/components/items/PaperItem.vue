<script lang="ts">
export default {
	props: [
        'pid',
        'pname',
        'psource',
        'pyear',
        'ptype',
        'level'
    ],
  emits: ['remove']
}
</script>

<template>
  <li>
    <div style="float: right; width: 20%; text-align: center;">
    <el-button plain type="danger" @click="$emit('remove')">删 除</el-button>
    </div>
    <div style="width: 80%;">
    <pre>
    论文编号：{{ pid }}
    论文名称：{{ pname }}
    论文来源：{{ psource }}
    论文年份：{{ pyear }}
    论文类别：{{ ptype }}
    论文级别：{{ level }}
    </pre>
    </div>
  </li>
</template>

<style scoped>
button{width: 5rem;height: 2.5rem;margin:.4rem 1rem}
</style>
