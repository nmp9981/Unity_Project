import { Client } from "@modelcontextprotocol/sdk/client/index.js";
import { StdioClientTransport } from "@modelcontextprotocol/sdk/client/stdio.js";

async function runUnity6Analyzer() {
  // 1. 서버 설정 (사용자님의 경로로 자동 세팅)
  const transport = new StdioClientTransport({
    command: "npx",
    args: [
      "-y",
      "@modelcontextprotocol/server-filesystem",
      "C:\\Users\\tybna\\SamSungEngineering2024_2" 
    ]
  });

  const client = new Client({
    name: "Unity6-Professional-Analyzer",
    version: "1.0.0"
  }, { capabilities: {} });

  try {
    await client.connect(transport);
    console.log("✅ MCP 서버 연결 성공! Unity 6 프로젝트를 분석합니다.");

    // [Step 1] 프로젝트의 패키지 구성을 읽어 배경 지식 확보
    const manifest = await client.callTool({
      name: "read_file",
      arguments: { path: "Packages/manifest.json" }
    });
    console.log("- 패키지 구성 읽기 완료.");

    // [Step 2] 분석하고 싶은 특정 파일 읽기 (예: PlayerController.cs 혹은 아무 파일명)
    // 실제 존재하는 파일명으로 바꿔보세요! 여기선 예시로 'Assets' 폴더 내 첫 파일을 찾는 로직입니다.
    const fileToAnalyze = "Assets/Scripts/ExampleScript.cs"; // <-- 분석하고 싶은 파일로 수정!
    
    try {
      const targetFile = await client.callTool({
        name: "read_file",
        arguments: { path: fileToAnalyze }
      });

      // [Step 3] 프롬프트 결합 (이게 바로 프롬프트 파트의 핵심입니다!)
      const systemPrompt = `
      너는 Unity 6 시니어 게임 개발자야. 
      현재 프로젝트 환경: ${manifest.content[0].text.substring(0, 500)}... (이하 생략)
      
      사용자가 제공하는 아래 코드를 분석해서 다음 기준에 따라 리포트를 작성해줘:
      1. Unity 6의 신규 그래픽스 API(Render Graph) 호환 여부
      2. 성능 최적화(ECS 호환성, 자가 동적 일괄 처리 등)
      3. 구버전 API(Deprecated) 사용 여부 확인
      `;

      console.log("\n============================================");
      console.log(`🔍 분석 대상: ${fileToAnalyze}`);
      console.log("============================================");
      
      // 실제로는 여기서 Claude API에 systemPrompt와 targetFile을 보냅니다.
      // 지금은 로직이 완성되었음을 보여주기 위해 읽어온 코드의 일부를 출력합니다.
      console.log("AI에게 보낼 데이터 준비 완료:");
      console.log(targetFile.content[0].text.substring(0, 300) + "...");

    } catch (err) {
      console.log(`\n⚠️  파일(${fileToAnalyze})을 찾을 수 없습니다. 실제 파일명으로 코드를 수정해 보세요!`);
    }

  } catch (error) {
    console.error("❌ 오류 발생:", error.message);
  }
}

runUnity6Analyzer();
