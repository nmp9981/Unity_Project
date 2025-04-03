--맵에 존재하는 몬스터 종류의 수
local monsterCountInMap = _DataService:GetRowCount("MonsterDataBase")
--맵위치
local parent = _EntityService:GetEntityByPath("/maps/map01")

for i=1,self.monsterMaxCount,1 do
	--소환할 몬스터 번호
	local monsterNum = math.random(monsterCountInMap)
	--소환 몬스터 아이디
	local id = _DataService:GetCell("MonsterDataBase",monsterNum,2)
	--소환할 몬스터 이름
	local name = _DataService:GetCell("MonsterDataBase",monsterNum,1)
	--소환 위치
	local Position = Vector3(math.random(-3,3),0,0)
	--소환
	_SpawnService:SpawnByModelId(id, name,Position,parent)
end
