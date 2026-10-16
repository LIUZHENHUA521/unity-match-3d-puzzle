  (func $j1511 (type 22) (param i32) (result f32)
    i32.const 4523864
    i32.load
    local.set 0
    local.get 0
    i32.eqz
    if  ;; label = @1
      i32.const 1584514
      call $j892
      local.set 0
      i32.const 4523864
      local.get 0
      i32.store
    end
    local.get 0
    call $redir_f
    return)
