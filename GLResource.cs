namespace izb_on
{
	interface GLResource
	{
		// setup and allocation of OpenGL resources
		void Assign();
		// cleanup and deallocation of OpenGL resources
		void Unassign();
	}
}