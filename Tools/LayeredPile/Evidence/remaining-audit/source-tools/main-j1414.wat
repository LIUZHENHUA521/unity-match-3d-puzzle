  (func $j1414 (type 22) (param i32) (result f32)
    i32.const 4523856
    i32.load
    local.set 0
    local.get 0
    i32.eqz
    if  ;; label = @1
      i32.const 1584443
      call $j892
      local.set 0
      i32.const 4523856
      local.get 0
      i32.store
    end
    local.get 0
    call $redir_f
    return)
