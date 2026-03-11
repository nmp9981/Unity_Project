import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StdioClientTransport } from "@modelcontextprotocol/sdk/client/stdio.js";

async function main() {
  const transport = new StdioClientTransport({
    command: "npx",
    args: [
      "-y",
      "@modelcontextprotocol/server-filesystem",
      "C:/Users/tybna/SamSungEngineering2024_2" // 사용자님의 유니티 프로젝트 경로
    ]
  });

  const client = new Client({
    name: "Unity-Project-Manager",
    version: "1.0.0"
  }, {
    capabilities: {}
  });

  try {
    console.log("MCP 서버에 연결 중...");
    await client.connect(transport);
    console.log("연결 성공!");

    // Assets 폴더 목록 가져오기 테스트
    const result = await client.callTool({
      name: "list_directory",
      arguments: { path: "C:\\Users\\tybna\\SamSungEngineering2024_2" }
    });

    console.log("--- Assets 폴더 목록 ---");
    console.log(result.content[0].text);

// 8. 사용 가능한 프롬프트 목록 확인
try {
  const prompts = await client.listPrompts();
  console.log("\n--- 사용 가능한 프롬프트 템플릿 ---");
  prompts.prompts.forEach(p => {
    console.log(`- ${p.name}: ${p.description}`);
  });
} catch (e) {
  console.log("\n(참고: 현재 서버는 별도의 프롬프트 템플릿을 제공하지 않습니다.)");
}

// 6. 사용 가능한 리소스 목록 확인
const resources = await client.listResources();
console.log("\n--- 사용 가능한 리소스 목록 ---");
resources.resources.forEach(res => {
  console.log(`- ${res.name} (${res.uri}): ${res.description}`);
});

// 7. 특정 리소스 내용 읽기 (예: 프로젝트 로그)
const resourceContent = await client.readResource({
  uri: "file:///C:/Users/tybna/SamSungEngineering2024_2/ProjectSettings/ProjectSettings.asset"
});
console.log("\n--- 리소스 내용 ---");
console.log(resourceContent.contents[0].text);
  } catch (error) {
    console.error("오류 발생:", error);
  }
}

main();
