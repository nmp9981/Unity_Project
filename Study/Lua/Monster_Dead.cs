self.IsDead = true
local stateComponent = self.Entity.StateComponent
if stateComponent then
	stateComponent:ChangeState("DEAD")
end

--현재 맵에 있는 몬스터 수가 줄어든다.
_MonsterSpawn.monsterCurCount-=1
_MonsterSpawn:MonsterSpawn()

local delayHide = function()
	self.Entity:SetVisible(false)
	self.Entity:SetEnable(false)
	
	if self.RespawnOn == false then
		self.Entity:Destroy()
	end
end

_TimerService:SetTimerOnce(delayHide, self.DestroyDelay)
