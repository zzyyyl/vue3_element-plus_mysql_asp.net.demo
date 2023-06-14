<script lang="ts">
export default {
  props: {},
  data() {
    return {
      hitokoto_text: '',
      hitokoto_from: '',
    }
  },
  mounted() {
    const hitokotoURL = 'https://v1.hitokoto.cn/?c=a&min_length=5&max_length=25'
    fetch(hitokotoURL)
      .then(response => response.json())
      .then(data => {
        this.hitokoto_text = data.hitokoto
        this.hitokoto_from = "——" + data.from
      })
      .catch(console.error)
  }
}


</script>

<template><div>
  <div class='blocktitle thick'>一言（ヒトコト）</div>
  <div class='blocktext hitokoto Plaintext'>
    <div style="font-size:1rem;line-height:1.5rem;">
      {{ hitokoto_text }}
    </div>
    <div style="text-align:right;margin-top:15px;font-size:0.9rem;color:#a3a4a5;">
      {{ hitokoto_from }}
    </div>
  </div>
</div></template>