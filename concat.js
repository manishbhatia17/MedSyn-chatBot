const concat = require('concat');
(async function build() {

  const files = [
    './dist/chat-bot/runtime.js',
    './dist/chat-bot/polyfills.js',
    './dist/chat-bot/main.js',
  ];
    await concat(files, './dist/chat-bot/helpdesk-chatbot_v1.js');
})();
