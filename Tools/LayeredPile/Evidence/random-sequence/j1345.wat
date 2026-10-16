  (func $j1345 (type 118) (param f32 f32 i32) (result f32)
    i32.const 4523200
    i32.load
    local.set 2
    local.get 2
    i32.eqz
    if  ;; label = @1
      i32.const 1576193
      call $j892
      local.set 2
      i32.const 4523200
      local.get 2
      i32.store
    end
    local.get 2
    local.get 0
    local.get 1
    call $redir_fff
    return)